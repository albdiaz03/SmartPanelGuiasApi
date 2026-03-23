using Microsoft.AspNetCore.Mvc;
using SmartPanelGuiasApi.Models;
using SmartPanelGuiasApi.Services;
using CryptSharp;

namespace SmartPanelGuiasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var token = _authService.Login(request.Correo, request.Password);
            if (token == null)
                return Unauthorized("Credenciales inválidas");
            return Ok(new LoginResponse
            {
                Ok = true,
                Mensaje = "Login correcto",
                Token = token
            });
        }

        [HttpGet("hash")]
        public IActionResult GenerarHash([FromQuery] string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new { hash });
        }
    }
}