using Atendimentos.Domain.Entities;

namespace Atendimentos.Application.Services
{
    public interface IPedidoItemService
    {
        Task<PedidoItem> CriarAsync(
            Guid pedidoId,
            int itemCardapioId,
            int quantidade,
            decimal precoMomento);

        Task<List<PedidoItem>>
            ObterTodosAsync();

        Task<List<PedidoItem>>
            ObterPorPedidoAsync(
                Guid pedidoId);
    }
}