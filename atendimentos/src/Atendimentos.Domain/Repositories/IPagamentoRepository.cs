using Atendimentos.Domain.Entities;

namespace Atendimentos.Domain.Repositories
{
    public interface IPagamentoRepository
    {
        Task<Pagamento> CriarAsync(
            Pagamento pagamento);

        Task<Pagamento?> ObterPorIdAsync(
            Guid id);

        Task<List<Pagamento>>
            ObterPorPedidoAsync(Guid pedidoId);

        Task<List<Pagamento>>
            ObterTodosAsync();

        Task AtualizarAsync(
            Pagamento pagamento);
    }
}
