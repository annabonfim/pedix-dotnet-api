using Microsoft.AspNetCore.Mvc;
using Atendimentos.Application.Services;
using Atendimentos.Api.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atendimentos.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        // POST: Criar cliente
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ClienteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _service.CriarAsync(dto.Nome, dto.Telefone);

            var resource = HateoasHelper.BuildResource(this, "clientes", cliente, cliente.Id);

            return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, resource);
        }

        // GET: Listar todos
        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _service.ObterTodosAsync();

            var result = clientes.Select(c =>
                HateoasHelper.BuildResource(this, "clientes", c, c.Id)
            );

            return Ok(result);
        }

        // GET: Buscar com filtro, paginação e ordenação
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? nome,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? sortBy = "Nome",
            [FromQuery] string? order = "asc")
        {
            var todos = await _service.ObterTodosAsync();
            var query = todos.AsQueryable();

            // Filtro
            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));

            // Ordenação dinâmica
            query = query.OrderByDynamic(sortBy, order);

            // Paginação
            var total = query.Count();
            var result = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // HATEOAS links da paginação
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/clientes/search";
            var links = new[]
            {
                new Link("self", $"{baseUrl}?page={page}&pageSize={pageSize}", "GET"),
                page > 1 ? new Link("prev", $"{baseUrl}?page={page-1}&pageSize={pageSize}", "GET") : null,
                (page * pageSize < total) ? new Link("next", $"{baseUrl}?page={page+1}&pageSize={pageSize}", "GET") : null
            }.Where(l => l != null);

            return Ok(new
            {
                items = result.Select(c => HateoasHelper.BuildResource(this, "clientes", c, c.Id)),
                pagination = new
                {
                    totalItems = total,
                    currentPage = page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                },
                links
            });
        }

        // GET: Buscar por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var cliente = await _service.ObterPorIdAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            var resource = HateoasHelper.BuildResource(this, "clientes", cliente, id);
            return Ok(resource);
        }

        // DELETE: Remover cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var cliente = await _service.ObterPorIdAsync(id);
            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            await _service.DeletarAsync(id);
            return NoContent();
        }
    }

    public class ClienteCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }
}
