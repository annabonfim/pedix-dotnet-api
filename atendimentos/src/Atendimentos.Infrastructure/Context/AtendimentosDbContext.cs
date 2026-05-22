using Microsoft.EntityFrameworkCore;

using Atendimentos.Domain.Entities;

namespace Atendimentos.Infrastructure.Context
{
    public class AtendimentosDbContext
        : DbContext
    {
        public AtendimentosDbContext(
            DbContextOptions<AtendimentosDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // 👤 CLIENTES
        // =====================================================
        public DbSet<Cliente> Clientes { get; set; }

        // =====================================================
        // 👨‍🍳 GARÇONS
        // =====================================================
        public DbSet<Garcom> Garcons { get; set; }

        // =====================================================
        // 🍽️ MESAS
        // =====================================================
        public DbSet<Mesa> Mesas { get; set; }

        // =====================================================
        // 🧾 COMANDAS
        // =====================================================
        public DbSet<Comanda> Comandas { get; set; }

        // =====================================================
        // 🔐 USUÁRIOS
        // =====================================================
        public DbSet<Usuario> Usuarios { get; set; }

        // =====================================================
        // 🧾 PEDIDOS
        // =====================================================
        public DbSet<Pedido> Pedidos { get; set; }

        // =====================================================
        // 📦 PEDIDO ITENS
        // =====================================================
        public DbSet<PedidoItem> PedidoItens { get; set; }

        // =====================================================
        // 💳 PAGAMENTOS
        // =====================================================
        public DbSet<Pagamento> Pagamentos { get; set; }

        // =====================================================
        // ⚙️ MODEL CREATING
        // =====================================================
        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =================================================
            // 👤 CLIENTE
            // =================================================
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("CLIENTES");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasColumnName("ID");

                entity.Property(c => c.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(c => c.Telefone)
                    .HasColumnName("TELEFONE")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // =================================================
            // 👨‍🍳 GARÇOM
            // =================================================
            modelBuilder.Entity<Garcom>(entity =>
            {
                entity.ToTable("GARCONS");

                entity.HasKey(g => g.Id);

                entity.Property(g => g.Id)
                    .HasColumnName("ID");

                entity.Property(g => g.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(g => g.Telefone)
                    .HasColumnName("TELEFONE")
                    .HasMaxLength(20);

                entity.Property(g => g.Matricula)
                    .HasColumnName("MATRICULA")
                    .HasMaxLength(50);
            });

            // =================================================
            // 🍽️ MESA
            // =================================================
            modelBuilder.Entity<Mesa>(entity =>
            {
                entity.ToTable("MESAS");

                entity.HasKey(m => m.Id);

                entity.Property(m => m.Id)
                    .HasColumnName("ID");

                entity.Property(m => m.Numero)
                    .HasColumnName("NUMERO")
                    .IsRequired();

                entity.Property(m => m.Capacidade)
                    .HasColumnName("CAPACIDADE")
                    .IsRequired();

                entity.Property(m => m.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(50);
            });

            // =================================================
            // 🧾 COMANDA
            // =================================================
            modelBuilder.Entity<Comanda>(entity =>
            {
                entity.ToTable("COMANDAS");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Id)
                    .HasColumnName("ID");

                entity.Property(c => c.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(50);

                entity.Property(c => c.ValorTotal)
                    .HasColumnName("VALORTOTAL")
                    .HasPrecision(10, 2);
            });

            // =================================================
            // 🔐 USUÁRIO
            // =================================================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                    .HasColumnName("ID");

                entity.Property(u => u.Nome)
                    .HasColumnName("NOME")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasColumnName("EMAIL")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(u => u.SenhaHash)
                    .HasColumnName("SENHAHASH")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(u => u.Telefone)
                    .HasColumnName("TELEFONE")
                    .HasMaxLength(20);

                entity.Property(u => u.Role)
                    .HasColumnName("ROLE")
                    .HasMaxLength(50);

                entity.Property(u => u.CPF)
                    .HasColumnName("CPF")
                    .HasMaxLength(14);

                entity.Property(u => u.Matricula)
                    .HasColumnName("MATRICULA")
                    .HasMaxLength(50);

                entity.Property(u => u.AdminKey)
                    .HasColumnName("ADMINKEY")
                    .HasMaxLength(100);
            });

            // =================================================
            // 🧾 PEDIDO
            // =================================================
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("PEDIDOS");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .HasColumnName("ID");

                entity.Property(p => p.ClienteId)
                    .HasColumnName("CLIENTEID");

                entity.Property(p => p.GarcomId)
                    .HasColumnName("GARCOMID");

                entity.Property(p => p.MesaId)
                    .HasColumnName("MESAID");

                entity.Property(p => p.DataPedido)
                    .HasColumnName("DATAPEDIDO");

                entity.Property(p => p.ValorTotal)
                    .HasColumnName("VALORTOTAL")
                    .HasPrecision(10, 2);

                entity.Property(p => p.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(50);
            });

            // =================================================
            // 📦 PEDIDO ITEM
            // =================================================
            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.ToTable("PEDIDO_ITENS");

                entity.HasKey(pi => pi.Id);

                entity.Property(pi => pi.Id)
                    .HasColumnName("ID");

                entity.Property(pi => pi.PedidoId)
                    .HasColumnName("PEDIDOID");

                entity.Property(pi => pi.NomeProduto)
                    .HasColumnName("NOMEPRODUTO")
                    .HasMaxLength(200);

                entity.Property(pi => pi.Quantidade)
                    .HasColumnName("QUANTIDADE");

                entity.Property(pi => pi.PrecoUnitario)
                    .HasColumnName("PRECOUNITARIO")
                    .HasPrecision(10, 2);

                entity.Property(pi => pi.Subtotal)
                    .HasColumnName("SUBTOTAL")
                    .HasPrecision(10, 2);
            });

            // =================================================
            // 💳 PAGAMENTO
            // =================================================
            modelBuilder.Entity<Pagamento>(entity =>
            {
                entity.ToTable("PAGAMENTOS");

                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .HasColumnName("ID");

                entity.Property(p => p.PedidoId)
                    .HasColumnName("PEDIDOID");

                entity.Property(p => p.Valor)
                    .HasColumnName("VALOR")
                    .HasPrecision(10, 2);

                entity.Property(p => p.MetodoPagamento)
                    .HasColumnName("METODOPAGAMENTO")
                    .HasMaxLength(50);

                entity.Property(p => p.Status)
                    .HasColumnName("STATUS")
                    .HasMaxLength(50);

                entity.Property(p => p.DataPagamento)
                    .HasColumnName("DATAPAGAMENTO");
            });
        }
    }
}