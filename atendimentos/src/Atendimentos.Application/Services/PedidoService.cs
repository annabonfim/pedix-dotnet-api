using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services
{
    public class PedidoService
        : IPedidoService
    {
        private readonly IPedidoRepository _repository;

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidoService(
            IPedidoRepository repository)
        {
            _repository = repository;
        }

        // =====================================================
        // ✅ CRIAR PEDIDO
        // =====================================================
        public async Task<Pedido>
            CriarAsync(
                Guid clienteId,
                Guid garcomId,
                Guid mesaId)
        {
            var pedido =
                new Pedido(
                    clienteId,
                    garcomId,
                    mesaId);

            return await _repository
                .CriarAsync(pedido);
        }

        // =====================================================
        // 📋 LISTAR PEDIDOS
        // =====================================================
        public async Task<List<Pedido>>
            ObterTodosAsync()
        {
            return await _repository
                .ObterTodosAsync();
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        public async Task<Pedido?>
            ObterPorIdAsync(Guid id)
        {
            return await _repository
                .ObterPorIdAsync(id);
        }
    }
}