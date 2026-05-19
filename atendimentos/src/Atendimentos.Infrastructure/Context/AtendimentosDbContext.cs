using Microsoft.EntityFrameworkCore;
using Atendimentos.Domain.Entities;

namespace Atendimentos.Infrastructure.Context
{
    public class AtendimentosDbContext : DbContext
    {
        public AtendimentosDbContext(
            DbContextOptions<AtendimentosDbContext> options)
            : base(options)
        {
        }

        // =====================================================
        // 📦 DBSETS
        // =====================================================

        public DbSet<Mesa> Mesas { get; set; }

        public DbSet<Garcom> Garcons { get; set; }

        public DbSet<Comanda> Comandas { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        // =====================================================
        // 🧩 MODEL CREATING
        // =====================================================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // 🪑 TABELA MESAS
            // =====================================================

            modelBuilder.Entity<Mesa>(entity =>
            {
                entity.ToTable("MESAS");

                entity.HasKey(m => m.Id);

                entity.HasIndex(m => m.Numero)
                    .IsUnique();

                entity.Property(m => m.Numero)
                    .IsRequired();

                entity.Property(m => m.Status)
                    .IsRequired();

                entity.Property(m => m.Capacidade);

                entity.Property(m => m.Localizacao)
                    .HasMaxLength(80);

                entity.Property(m => m.QrCode)
                    .HasMaxLength(256);

                entity.Property(m => m.CreatedAt)
                    .IsRequired();

                entity.Property(m => m.UpdatedAt)
                    .IsRequired();

                entity.Property(m => m.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .IsRequired();
            });

            // =====================================================
            // 🧑‍🍳 TABELA GARCONS
            // =====================================================

            modelBuilder.Entity<Garcom>(entity =>
            {
                entity.ToTable("GARCONS");

                entity.HasKey(g => g.Id);

                entity.Property(g => g.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(g => g.Matricula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.Telefone)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.DataContratacao)
                    .IsRequired();

                entity.Property(g => g.Ativo)
                    .IsRequired();
            });

            // =====================================================
            // 🧾 TABELA COMANDAS
            // =====================================================

            modelBuilder.Entity<Comanda>(entity =>
            {
                entity.ToTable("COMANDAS");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Status)
                    .IsRequired();

                entity.Property(c => c.DataHoraAbertura)
                    .IsRequired();

                entity.Property(c => c.ValorTotal)
                    .HasColumnType("DECIMAL(10,2)");

                entity.HasOne<Mesa>()
                    .WithMany()
                    .HasForeignKey(c => c.MesaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Garcom>()
                    .WithMany()
                    .HasForeignKey(c => c.GarcomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Cliente>()
                    .WithMany()
                    .HasForeignKey(c => c.ClienteId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // =====================================================
            // 👤 TABELA CLIENTES
            // =====================================================

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("CLIENTES");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(c => c.Telefone)
                    .HasMaxLength(20);

                entity.Property(c => c.DataCadastro)
                    .IsRequired();
            });

            // =====================================================
            // 🔐 TABELA USUARIOS
            // =====================================================

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIOS");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                    .HasColumnName("ID");

                entity.Property(u => u.Nome)
                    .HasColumnName("NOME")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.Email)
                    .HasColumnName("EMAIL")
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Telefone)
                    .HasColumnName("TELEFONE")
                    .HasMaxLength(20);

                entity.Property(u => u.SenhaHash)
                    .HasColumnName("SENHA_HASH")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.DataNascimento)
                    .HasColumnName("DATA_NASCIMENTO")
                    .IsRequired();

                entity.Property(u => u.DataCriacao)
                    .HasColumnName("DATA_CRIACAO")
                    .IsRequired();

                entity.Property(u => u.Ativo)
                    .HasColumnName("ATIVO")
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasColumnName("ROLE")
                    .IsRequired()
                    .HasMaxLength(30);
            });
        }
    }
}