using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Enums;

using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services
{
    public class PedidoService
        : IPedidoService
    {
        private readonly IPedidoRepository _repository;
        private readonly IMesaRepository _mesaRepository;

        // Status válidos pra um pedido. Recusa qualquer outro valor.
        // FLUXO:
        //   ABERTO → EM_PREPARO → PRONTO → ENTREGUE   (transições do garçom)
        //   ENTREGUE → FINALIZADO                     (transição via pagamento)
        //   qualquer → CANCELADO                      (cancelamento)
        // ENTREGUE = comida na mesa, conta em aberto.
        // FINALIZADO = pagamento aprovado, conta fechada (libera mesa).
        private static readonly HashSet<string> _statusValidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "ABERTO",
                "EM_PREPARO",
                "PRONTO",
                "ENTREGUE",
                "FINALIZADO",
                "CANCELADO",
            };

        // Status terminais (pedido não conta mais como "ativo" pra ocupar mesa).
        // ENTREGUE NÃO é terminal — cliente ainda precisa pagar pra fechar a conta.
        private static readonly HashSet<string> _statusTerminais =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "FINALIZADO",
                "CANCELADO",
            };

        // Status que devem disparar a checagem de liberação de mesa.
        // CANCELADO NÃO entra: cliente pode ter cancelado mas continuar na mesa
        // querendo pedir outra coisa; só FINALIZADO (= pagamento aprovado)
        // significa que a conta fechou de verdade.
        private static readonly HashSet<string> _statusLiberaMesa =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "FINALIZADO",
            };

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PedidoService(
            IPedidoRepository repository,
            IMesaRepository mesaRepository)
        {
            _repository = repository;
            _mesaRepository = mesaRepository;
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

            var criado = await _repository
                .CriarAsync(pedido);

            // Mesa com comanda aberta deixa de estar livre.
            await OcuparMesaAsync(mesaId);

            return criado;
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

        // =====================================================
        // 🔄 ATUALIZAR STATUS
        // =====================================================
        public async Task<Pedido?>
            AtualizarStatusAsync(
                Guid id,
                string status)
        {
            if (string.IsNullOrWhiteSpace(status)
                || !_statusValidos.Contains(status))
            {
                throw new ArgumentException(
                    $"Status inválido: '{status}'. " +
                    $"Válidos: {string.Join(", ", _statusValidos)}.");
            }

            var pedido = await _repository
                .ObterPorIdAsync(id);

            if (pedido == null) return null;

            var statusNovo = status.ToUpperInvariant();
            pedido.AlterarStatus(statusNovo);

            await _repository.AtualizarAsync(pedido);

            // ATENÇÃO: NÃO liberar mesa aqui mesmo se status for ENTREGUE/FINALIZADO.
            //   ENTREGUE = garçom levou a comida; conta segue em aberto.
            //   FINALIZADO = pagamento aprovado; mesa pode ser liberada.
            //   A liberação de mesa só é disparada por PagamentoService após
            //   aprovar pagamento (chama LiberarMesaSeOciosaAsync explicitamente).

            return pedido;
        }

        // =====================================================
        // 🔍 LISTAR POR CLIENTE
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorClienteAsync(Guid clienteId)
        {
            return await _repository
                .ObterPorClienteAsync(clienteId);
        }

        // =====================================================
        // 🔍 LISTAR POR MESA
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorMesaAsync(Guid mesaId)
        {
            return await _repository
                .ObterPorMesaAsync(mesaId);
        }

        // =====================================================
        // 🔍 LISTAR POR GARÇOM
        // =====================================================
        public async Task<List<Pedido>>
            ObterPorGarcomAsync(Guid garcomId)
        {
            return await _repository
                .ObterPorGarcomAsync(garcomId);
        }

        // =====================================================
        // 🔒 HELPERS DE MESA
        // =====================================================
        private async Task OcuparMesaAsync(Guid mesaId)
        {
            var mesa = await _mesaRepository.ObterPorIdAsync(mesaId);
            if (mesa == null) return;
            if (mesa.Status == MesaStatus.Ocupada) return;
            mesa.AlterarStatus(MesaStatus.Ocupada);
            await _mesaRepository.AtualizarAsync(mesa);
        }

        // Público (parte da IPedidoService) pra ser chamado pelo PagamentoService
        // depois de aprovar uma conta. Faz nada se ainda existe pedido ativo
        // (outro cliente na mesma mesa, por exemplo).
        public async Task LiberarMesaSeOciosaAsync(Guid mesaId)
        {
            var pedidosDaMesa = await _repository.ObterPorMesaAsync(mesaId);
            // "Ativo" = qualquer pedido que ainda não foi entregue NEM cancelado.
            // Se a mesa só tem entregues/cancelados, ela pode ser liberada.
            var temAtivo = pedidosDaMesa.Any(p =>
                !_statusTerminais.Contains(p.Status));
            if (temAtivo) return;

            var mesa = await _mesaRepository.ObterPorIdAsync(mesaId);
            if (mesa == null) return;
            if (mesa.Status == MesaStatus.Livre) return;
            mesa.AlterarStatus(MesaStatus.Livre);
            await _mesaRepository.AtualizarAsync(mesa);
        }
    }
}
