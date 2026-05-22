namespace Atendimentos.Domain.Entities
{
    public class Pagamento
    {
        public Guid Id { get; private set; }

        public Guid PedidoId { get; private set; }

        public decimal Valor { get; private set; }

        public string MetodoPagamento { get; private set; }

        public string Status { get; private set; }

        public DateTime DataPagamento { get; private set; }

        // ==========================================
        // 🏗️ CONSTRUTOR
        // ==========================================
        public Pagamento(
            Guid pedidoId,
            decimal valor,
            string metodoPagamento)
        {
            Id = Guid.NewGuid();

            PedidoId = pedidoId;

            Valor = valor;

            MetodoPagamento = metodoPagamento;

            Status = "PENDENTE";

            DataPagamento = DateTime.UtcNow;
        }

        // ==========================================
        // ✅ APROVAR PAGAMENTO
        // ==========================================
        public void Aprovar()
        {
            Status = "APROVADO";
        }

        // ==========================================
        // ❌ RECUSAR PAGAMENTO
        // ==========================================
        public void Recusar()
        {
            Status = "RECUSADO";
        }

        protected Pagamento()
        {
        }
    }
}