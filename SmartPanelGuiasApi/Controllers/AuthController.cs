using Microsoft.AspNetCore.Mvc;
using SmartPanelGuiasApi.Models;
using SmartPanelGuiasApi.Services;

namespace SmartPanelGuiasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponse> Login(LoginRequest request)
        {
            if (_auth.ValidarUsuario(request.Usuario, request.Password))
            {
                return Ok(new LoginResponse
                {
                    Ok = true,
                    Mensaje = "Login correcto",
                    Nombre = request.Usuario
                });
            }

            return Unauthorized(new LoginResponse
            {
                Ok = false,
                Mensaje = "Usuario o contraseña incorrectos"
            });
        }
    }
}
