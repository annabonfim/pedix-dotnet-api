using Atendimentos.Domain.Entities;

namespace Atendimentos.Domain.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> CriarAsync(
            Pedido pedido);

        Task<List<Pedido>> ObterTodosAsync();

        Task<Pedido?> ObterPorIdAsync(
            Guid id);
    }
}