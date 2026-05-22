using System;

namespace Atendimentos.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; private set; }

        public string Nome { get; private set; }

        public string Email { get; private set; }

        public string SenhaHash { get; private set; }

        public string Telefone { get; private set; }

        public DateTime DataNascimento { get; private set; }

        // =====================================================
        // 🪪 CPF
        // =====================================================
        public string? CPF { get; private set; }

        // =====================================================
        // 🆔 MATRÍCULA
        // =====================================================
        public string? Matricula { get; private set; }

        // =====================================================
        // 🔑 ADMIN KEY
        // =====================================================
        public string? AdminKey { get; private set; }

        // =====================================================
        // 🔐 ROLE
        // =====================================================
        public string Role { get; private set; }

        // =====================================================
        // ✅ ATIVO
        // =====================================================
        public bool Ativo { get; private set; }

        // =====================================================
        // 📅 DATA CRIAÇÃO
        // =====================================================
        public DateTime DataCriacao { get; private set; }

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public Usuario(
            string nome,
            string email,
            string senhaHash,
            string telefone,
            DateTime dataNascimento,
            string role,
            string? cpf = null,
            string? matricula = null,
            string? adminKey = null)
        {
            Id = Guid.NewGuid();

            Nome = nome;

            Email = email;

            SenhaHash = senhaHash;

            Telefone = telefone;

            DataNascimento = dataNascimento;

            Role = role;

            CPF = cpf;

            Matricula = matricula;

            AdminKey = adminKey;

            Ativo = true;

            DataCriacao = DateTime.UtcNow;
        }

        // =====================================================
        // 🧱 EF CORE
        // =====================================================
        protected Usuario()
        {
        }
    }
}