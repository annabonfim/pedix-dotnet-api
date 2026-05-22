using Microsoft.AspNetCore.Mvc;

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
    }
}