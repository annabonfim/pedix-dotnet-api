using Atendimentos.Domain.Entities;

namespace Atendimentos.Application.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CriarAsync(
            Guid clienteId,
            Guid garcomId,
            Guid mesaId);

        Task<List<Pedido>>
            ObterTodosAsync();

        Task<Pedido?>
            ObterPorIdAsync(Guid id);

        Task<Pedido?>
            AtualizarStatusAsync(
                Guid id,
                string status);

        Task<List<Pedido>>
            ObterPorClienteAsync(Guid clienteId);

        Task<List<Pedido>>
            ObterPorMesaAsync(Guid mesaId);

        Task<List<Pedido>>
            ObterPorGarcomAsync(Guid garcomId);

        // Libera a mesa SE ela não tem mais nenhum pedido ativo.
        // Chamada explicitamente pelo PagamentoService após aprovar conta,
        // pra não fazer parte do fluxo de AtualizarStatus (ENTREGUE não
        // significa "conta paga", só "comida na mesa").
        Task LiberarMesaSeOciosaAsync(Guid mesaId);
    }
}
