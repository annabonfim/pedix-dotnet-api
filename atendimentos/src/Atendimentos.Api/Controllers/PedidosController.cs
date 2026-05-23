using Microsoft.AspNetCore.Mvc;

using Atendimentos.Application.DTOs;
using Atendimentos.Application.Services;

namespace Atendimentos.Api.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController
        : ControllerBase
    {
        private readonly IPedidoService _service;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidosController(
            IPedidoService service)
        {
            _service = service;
        }

        // =====================================================
        // ✅ CRIAR PEDIDO
        // =====================================================
        [HttpPost]
        public async Task<IActionResult>
            CriarPedido(
                Guid clienteId,
                Guid garcomId,
                Guid mesaId)
        {
            var pedido =
                await _service.CriarAsync(
                    clienteId,
                    garcomId,
                    mesaId);

            return Ok(pedido);
        }

        // =====================================================
        // 📋 LISTAR PEDIDOS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult>
            ObterTodos()
        {
            var pedidos =
                await _service.ObterTodosAsync();

            return Ok(pedidos);
        }

        // =====================================================
        // 🔍 BUSCAR PEDIDO POR ID
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult>
            ObterPorId(Guid id)
        {
            var pedido =
                await _service.ObterPorIdAsync(id);

            if (pedido == null)
            {
                return NotFound(new
                {
                    message =
                        "Pedido não encontrado."
                });
            }

            return Ok(pedido);
        }

        // =====================================================
        // 🔄 ATUALIZAR STATUS DO PEDIDO
        // =====================================================
        [HttpPut("{id}/status")]
        public async Task<IActionResult>
            AtualizarStatus(
                Guid id,
                [FromBody] AtualizarStatusPedidoDto dto)
        {
            try
            {
                var pedido = await _service
                    .AtualizarStatusAsync(id, dto.Status);

                if (pedido == null)
                {
                    return NotFound(new
                    {
                        message = "Pedido não encontrado."
                    });
                }

                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =====================================================
        // 🔍 LISTAR POR CLIENTE
        // =====================================================
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult>
            ObterPorCliente(Guid clienteId)
        {
            var pedidos = await _service
                .ObterPorClienteAsync(clienteId);

            return Ok(pedidos);
        }

        // =====================================================
        // 🔍 LISTAR POR MESA
        // =====================================================
        [HttpGet("mesa/{mesaId}")]
        public async Task<IActionResult>
            ObterPorMesa(Guid mesaId)
        {
            var pedidos = await _service
                .ObterPorMesaAsync(mesaId);

            return Ok(pedidos);
        }

        // =====================================================
        // 🔍 LISTAR POR GARÇOM
        // =====================================================
        [HttpGet("garcom/{garcomId}")]
        public async Task<IActionResult>
            ObterPorGarcom(Guid garcomId)
        {
            var pedidos = await _service
                .ObterPorGarcomAsync(garcomId);

            return Ok(pedidos);
        }
    }
}
