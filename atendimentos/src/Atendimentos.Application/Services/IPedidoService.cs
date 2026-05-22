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
    }
}