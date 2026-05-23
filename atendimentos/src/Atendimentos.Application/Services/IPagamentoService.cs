using Atendimentos.Domain.Entities;

namespace Atendimentos.Application.Services
{
    public interface IPagamentoService
    {
        Task<Pagamento> CriarAsync(
            Guid pedidoId,
            decimal valor,
            string metodoPagamento);

        Task<Pagamento?> ObterPorIdAsync(
            Guid id);

        Task<List<Pagamento>>
            ObterPorPedidoAsync(Guid pedidoId);

        Task<List<Pagamento>>
            ObterTodosAsync();

        Task<Pagamento?>
            AprovarAsync(Guid id);

        Task<Pagamento?>
            RecusarAsync(Guid id);
    }
}
