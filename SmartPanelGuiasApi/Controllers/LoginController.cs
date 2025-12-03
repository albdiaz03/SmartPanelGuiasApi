using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartPanelGuiasApi.Models;

namespace SmartPanelGuiasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Usuario == "admin" && request.Password == "1234")
            {
                var token = GenerateJwtToken(request.Usuario);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string usuario)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, usuario) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ESTA_ES_LA_LLAVE_SECRETA_123"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "smartpanel",
                audience: "smartpanel",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
