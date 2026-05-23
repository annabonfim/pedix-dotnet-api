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

        Task AtualizarAsync(
            Pedido pedido);

        Task<List<Pedido>> ObterPorClienteAsync(
            Guid clienteId);

        Task<List<Pedido>> ObterPorMesaAsync(
            Guid mesaId);

        Task<List<Pedido>> ObterPorGarcomAsync(
            Guid garcomId);
    }
}
