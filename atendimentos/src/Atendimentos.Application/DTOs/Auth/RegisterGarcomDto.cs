using System.ComponentModel.DataAnnotations;

namespace Atendimentos.Application.DTOs.Auth
{
    public class RegisterGarcomDto
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }

        [Required]
        public string Telefone { get; set; }

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string CPF { get; set; }

        [Required]
        public string Matricula { get; set; }
    }
}