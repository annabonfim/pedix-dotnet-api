namespace Atendimentos.Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; private set; }

        public Guid ClienteId { get; private set; }

        public Guid GarcomId { get; private set; }

        public Guid MesaId { get; private set; }

        public DateTime DataPedido { get; private set; }

        public decimal ValorTotal { get; private set; }

        public string Status { get; private set; }

        // Observação livre do cliente (ex: "sem cebola", "bem passado").
        // Nullable porque pedido pode ser criado sem nenhuma anotação.
        public string? Observacao { get; private set; }

        // ==========================================
        // 🏗️ CONSTRUTOR
        // ==========================================
        public Pedido(
            Guid clienteId,
            Guid garcomId,
            Guid mesaId,
            string? observacao = null)
        {
            Id = Guid.NewGuid();

            ClienteId = clienteId;

            GarcomId = garcomId;

            MesaId = mesaId;

            DataPedido = DateTime.UtcNow;

            ValorTotal = 0;

            Status = "ABERTO";

            Observacao = string.IsNullOrWhiteSpace(observacao)
                ? null
                : observacao.Trim();
        }

        // ==========================================
        // 💰 ATUALIZA VALOR
        // ==========================================
        public void AtualizarValorTotal(
            decimal valor)
        {
            ValorTotal = valor;
        }

        // ==========================================
        // 🔄 ALTERA STATUS
        // ==========================================
        public void AlterarStatus(
            string status)
        {
            Status = status;
        }

        // ==========================================
        // 📝 ALTERA OBSERVAÇÃO
        // ==========================================
        public void AlterarObservacao(
            string? observacao)
        {
            Observacao = string.IsNullOrWhiteSpace(observacao)
                ? null
                : observacao.Trim();
        }

        protected Pedido()
        {
        }
    }
}