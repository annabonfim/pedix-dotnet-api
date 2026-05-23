using Microsoft.AspNetCore.Mvc;

using Atendimentos.Application.DTOs;
using Atendimentos.Application.Services;

namespace Atendimentos.Api.Controllers
{
    [ApiController]
    [Route("api/pagamentos")]
    public class PagamentosController
        : ControllerBase
    {
        private readonly IPagamentoService _service;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PagamentosController(
            IPagamentoService service)
        {
            _service = service;
        }

        // =====================================================
        // ✅ CRIAR PAGAMENTO
        // =====================================================
        [HttpPost]
        public async Task<IActionResult>
            Criar([FromBody] CriarPagamentoDto dto)
        {
            try
            {
                var pagamento = await _service
                    .CriarAsync(
                        dto.PedidoId,
                        dto.Valor,
                        dto.MetodoPagamento);

                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = pagamento.Id },
                    pagamento);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult>
            ObterPorId(Guid id)
        {
            var pagamento = await _service
                .ObterPorIdAsync(id);

            if (pagamento == null)
            {
                return NotFound(new
                {
                    message = "Pagamento não encontrado."
                });
            }

            return Ok(pagamento);
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        [HttpGet("pedido/{pedidoId}")]
        public async Task<IActionResult>
            ObterPorPedido(Guid pedidoId)
        {
            var pagamentos = await _service
                .ObterPorPedidoAsync(pedidoId);

            return Ok(pagamentos);
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult>
            ObterTodos()
        {
            var pagamentos = await _service
                .ObterTodosAsync();

            return Ok(pagamentos);
        }

        // =====================================================
        // ✅ APROVAR PAGAMENTO
        // =====================================================
        [HttpPut("{id}/aprovar")]
        public async Task<IActionResult>
            Aprovar(Guid id)
        {
            var pagamento = await _service
                .AprovarAsync(id);

            if (pagamento == null)
            {
                return NotFound(new
                {
                    message = "Pagamento não encontrado."
                });
            }

            return Ok(pagamento);
        }

        // =====================================================
        // ❌ RECUSAR PAGAMENTO
        // =====================================================
        [HttpPut("{id}/recusar")]
        public async Task<IActionResult>
            Recusar(Guid id)
        {
            var pagamento = await _service
                .RecusarAsync(id);

            if (pagamento == null)
            {
                return NotFound(new
                {
                    message = "Pagamento não encontrado."
                });
            }

            return Ok(pagamento);
        }
    }
}
