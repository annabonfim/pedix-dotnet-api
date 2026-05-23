using Atendimentos.Domain.Entities;

using Atendimentos.Domain.Repositories;

namespace Atendimentos.Application.Services
{
    public class PagamentoService
        : IPagamentoService
    {
        private readonly IPagamentoRepository _repository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoService _pedidoService;

        // Métodos de pagamento aceitos
        private static readonly HashSet<string> _metodosValidos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "PIX",
                "CREDITO",
                "DEBITO",
                "DINHEIRO",
            };

        // Status que já são terminais — não viram FINALIZADO de novo.
        // ENTREGUE NÃO é terminal: cliente já recebeu a comida mas ainda não
        // pagou; quando pagar, esses pedidos ENTREGUE viram FINALIZADO.
        private static readonly HashSet<string> _statusTerminais =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "FINALIZADO",
                "CANCELADO",
            };

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public PagamentoService(
            IPagamentoRepository repository,
            IPedidoRepository pedidoRepository,
            IPedidoService pedidoService)
        {
            _repository = repository;
            _pedidoRepository = pedidoRepository;
            _pedidoService = pedidoService;
        }

        // =====================================================
        // ✅ CRIAR PAGAMENTO
        // =====================================================
        public async Task<Pagamento>
            CriarAsync(
                Guid pedidoId,
                decimal valor,
                string metodoPagamento)
        {
            if (pedidoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "PedidoId é obrigatório.");
            }

            if (valor <= 0)
            {
                throw new ArgumentException(
                    "Valor do pagamento deve ser maior que zero.");
            }

            if (string.IsNullOrWhiteSpace(metodoPagamento)
                || !_metodosValidos.Contains(metodoPagamento))
            {
                throw new ArgumentException(
                    $"Método de pagamento inválido. " +
                    $"Válidos: {string.Join(", ", _metodosValidos)}.");
            }

            var pagamento =
                new Pagamento(
                    pedidoId,
                    valor,
                    metodoPagamento.ToUpperInvariant());

            return await _repository
                .CriarAsync(pagamento);
        }

        // =====================================================
        // 🔍 BUSCAR POR ID
        // =====================================================
        public async Task<Pagamento?>
            ObterPorIdAsync(Guid id)
        {
            return await _repository
                .ObterPorIdAsync(id);
        }

        // =====================================================
        // 🔍 LISTAR POR PEDIDO
        // =====================================================
        public async Task<List<Pagamento>>
            ObterPorPedidoAsync(Guid pedidoId)
        {
            return await _repository
                .ObterPorPedidoAsync(pedidoId);
        }

        // =====================================================
        // 📋 LISTAR TODOS
        // =====================================================
        public async Task<List<Pagamento>>
            ObterTodosAsync()
        {
            return await _repository
                .ObterTodosAsync();
        }

        // =====================================================
        // ✅ APROVAR PAGAMENTO
        // =====================================================
        // Aprovar pagamento = fechar a conta do cliente na mesa:
        //   1. Pagamento vira APROVADO
        //   2. Todos os pedidos não-cancelados do cliente naquela mesa viram FINALIZADO
        //      (inclusive os já ENTREGUE — agora viram FINALIZADO = pago + fechado)
        //   3. Mesa volta pra LIVRE (se não tem mais pedido ativo de ninguém)
        //
        // ENTREGUE sozinho NÃO libera a mesa — o garçom pode entregar comida
        // sem a conta estar paga. Só PAGAMENTO APROVADO fecha a mesa.
        public async Task<Pagamento?>
            AprovarAsync(Guid id)
        {
            var pagamento = await _repository
                .ObterPorIdAsync(id);

            if (pagamento == null) return null;

            pagamento.Aprovar();

            await _repository.AtualizarAsync(pagamento);

            var mesaId = await FecharContaAsync(pagamento.PedidoId);

            // Depois de marcar os pedidos como FINALIZADO, tenta liberar a mesa.
            // Se ainda há outro cliente com pedido ativo na mesma mesa,
            // o LiberarMesaSeOciosaAsync mantém ela ocupada.
            if (mesaId.HasValue)
            {
                await _pedidoService
                    .LiberarMesaSeOciosaAsync(mesaId.Value);
            }

            return pagamento;
        }

        // =====================================================
        // 🧾 FECHAR CONTA (helper)
        // =====================================================
        // Retorna o mesaId pra o caller usar na liberação da mesa.
        private async Task<Guid?> FecharContaAsync(Guid pedidoIdPago)
        {
            var pedidoBase = await _pedidoRepository
                .ObterPorIdAsync(pedidoIdPago);

            if (pedidoBase == null) return null;

            var pedidosDoCliente = await _pedidoRepository
                .ObterPorClienteAsync(pedidoBase.ClienteId);

            var pedidosParaFechar = pedidosDoCliente
                .Where(p => p.MesaId == pedidoBase.MesaId)
                .Where(p => !_statusTerminais.Contains(p.Status));

            foreach (var pedido in pedidosParaFechar)
            {
                await _pedidoService
                    .AtualizarStatusAsync(pedido.Id, "FINALIZADO");
            }

            return pedidoBase.MesaId;
        }

        // =====================================================
        // ❌ RECUSAR PAGAMENTO
        // =====================================================
        public async Task<Pagamento?>
            RecusarAsync(Guid id)
        {
            var pagamento = await _repository
                .ObterPorIdAsync(id);

            if (pagamento == null) return null;

            pagamento.Recusar();

            await _repository.AtualizarAsync(pagamento);

            return pagamento;
        }
    }
}
