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

        // =====================================================
        // 🏗️ CONSTRUTOR
        // =====================================================
        public AuthController(
            IAuthService service)
        {
            _service = service;
        }

        // =====================================================
        // 👤 CADASTRO CLIENTE
        // =====================================================
        [HttpPost("register-cliente")]
        public async Task<IActionResult> RegisterCliente(
            RegisterClienteDto dto)
        {
            try
            {
                var usuario =
                    await _service
                        .RegistrarClienteAsync(dto);

                return Ok(new
                {
                    message =
                        "Cliente cadastrado com sucesso.",

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

        // =====================================================
        // 🧑‍🍳 CADASTRO GARÇOM
        // =====================================================
        [HttpPost("register-garcom")]
        public async Task<IActionResult> RegisterGarcom(
            RegisterGarcomDto dto)
        {
            try
            {
                var usuario =
                    await _service
                        .RegistrarGarcomAsync(dto);

                return Ok(new
                {
                    message =
                        "Garçom cadastrado com sucesso.",

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

        // =====================================================
        // 👑 CADASTRO ADMIN
        // =====================================================
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(
            RegisterAdminDto dto)
        {
            try
            {
                var usuario =
                    await _service
                        .RegistrarAdminAsync(dto);

                return Ok(new
                {
                    message =
                        "Administrador cadastrado com sucesso.",

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

        // =====================================================
        // 🔐 LOGIN JWT
        // =====================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            try
            {
                var token =
                    await _service.LoginAsync(dto);

                return Ok(new
                {
                    message =
                        "Login realizado com sucesso.",

                    token
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