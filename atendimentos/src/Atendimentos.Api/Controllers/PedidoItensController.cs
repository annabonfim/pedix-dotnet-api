using Microsoft.AspNetCore.Mvc;

using Atendimentos.Application.Services;

namespace Atendimentos.Api.Controllers
{
    [ApiController]
    [Route("api/pedido-itens")]
    public class PedidoItensController
        : ControllerBase
    {
        private readonly IPedidoItemService _service;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidoItensController(
            IPedidoItemService service)
        {
            _service = service;
        }

        // =====================================================
        // ✅ CRIAR ITEM PEDIDO
        // =====================================================
        [HttpPost]
        public async Task<IActionResult>
            Criar(
                Guid pedidoId,
                int itemCardapioId,
                int quantidade,
                decimal precoMomento)
        {
            var item =
                await _service.CriarAsync(
                    pedidoId,
                    itemCardapioId,
                    quantidade,
                    precoMomento);

            return Ok(item);
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult>
            ObterTodos()
        {
            var itens =
                await _service.ObterTodosAsync();

            return Ok(itens);
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        [HttpGet("pedido/{pedidoId}")]
        public async Task<IActionResult>
            ObterPorPedido(Guid pedidoId)
        {
            var itens =
                await _service
                    .ObterPorPedidoAsync(
                        pedidoId);

            return Ok(itens);
        }
    }
}