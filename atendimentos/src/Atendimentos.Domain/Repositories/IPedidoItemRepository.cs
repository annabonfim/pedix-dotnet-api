using Atendimentos.Domain.Entities;

namespace Atendimentos.Domain.Repositories
{
    public interface IPedidoItemRepository
    {
        Task<PedidoItem> CriarAsync(
            PedidoItem item);

        Task<List<PedidoItem>>
            ObterTodosAsync();

        Task<List<PedidoItem>>
            ObterPorPedidoAsync(
                Guid pedidoId);
    }
}