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

        public string Role { get; private set; }

        public bool Ativo { get; private set; }

        public DateTime DataCriacao { get; private set; }

        public Usuario(
            string nome,
            string email,
            string senhaHash,
            string telefone,
            DateTime dataNascimento,
            string role)
        {
            Id = Guid.NewGuid();

            Nome = nome;

            Email = email;

            SenhaHash = senhaHash;

            Telefone = telefone;

            DataNascimento = dataNascimento;

            Role = role;

            Ativo = true;

            DataCriacao = DateTime.UtcNow;
        }

        protected Usuario()
        {
        }
    }
}