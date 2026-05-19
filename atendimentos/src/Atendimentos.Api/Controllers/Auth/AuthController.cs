using Microsoft.AspNetCore.Mvc;
using Atendimentos.Application.DTOs.Auth;
using Atendimentos.Application.Services.Auth;

namespace Atendimentos.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register-cliente")]
        public async Task<IActionResult> RegisterCliente(
            RegisterClienteDto dto)
        {
            try
            {
                var usuario =
                    await _service.RegistrarClienteAsync(dto);

                return Ok(new
                {
                    message = "Cliente cadastrado com sucesso.",
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }
    }
}