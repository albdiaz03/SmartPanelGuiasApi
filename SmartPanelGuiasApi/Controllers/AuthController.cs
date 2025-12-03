using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (req.Usuario != "admin" || req.Password != "1234")
            return Unauthorized();

        var key = Encoding.UTF8.GetBytes("ESTA_ES_LA_LLAVE_SECRETA_123");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, req.Usuario)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "smartpanel",
            Audience = "smartpanel",
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(tokenHandler.WriteToken(token));
    }
}

public class LoginRequest
{
    public string Usuario { get; set; }
    public string Password { get; set; }
}
