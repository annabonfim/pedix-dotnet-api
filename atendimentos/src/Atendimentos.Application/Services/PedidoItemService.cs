using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services
{
    public class PedidoItemService
        : IPedidoItemService
    {
        private readonly IPedidoItemRepository _repository;

        public PedidoItemService(
            IPedidoItemRepository repository)
        {
            _repository = repository;
        }

        // =====================================================
        // ✅ CRIAR ITEM PEDIDO
        // =====================================================
        public async Task<PedidoItem>
            CriarAsync(
                Guid pedidoId,
                int itemCardapioId,
                int quantidade,
                decimal precoMomento)
        {
            var item =
                new PedidoItem(
                    pedidoId,
                    itemCardapioId,
                    quantidade,
                    precoMomento);

            return await _repository
                .CriarAsync(item);
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        public async Task<List<PedidoItem>>
            ObterTodosAsync()
        {
            return await _repository
                .ObterTodosAsync();
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        public async Task<List<PedidoItem>>
            ObterPorPedidoAsync(
                Guid pedidoId)
        {
            return await _repository
                .ObterPorPedidoAsync(
                    pedidoId);
        }
    }
}