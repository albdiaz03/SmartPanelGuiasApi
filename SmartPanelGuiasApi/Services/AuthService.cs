using Microsoft.Data.SqlClient;
using SmartPanelGuiasApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartPanelGuiasApi.Conexion;   // 🔥 ESTE using ES CLAVE
using CryptSharp;

namespace SmartPanelGuiasApi.Services
{
    public class AuthService
    {
        private readonly string _key = "ESTA_ES_MI_CLAVE_SUPER_SECRETA_2026";
        private readonly DatabaseConnection _conexion;

        // Inyectamos la conexión desde Program.cs
        public AuthService(DatabaseConnection conexion)
        {
            _conexion = conexion;
        }

        public string Login(string correo, string password)
        {
            Usuario usuario = null;

            // Usamos la instancia _conexion
            using (var conn = _conexion.GetSmartPanelConnection())
            {
                conn.Open();

                string query = @"
SELECT u.*, p.nombre_pri AS rolNombre
FROM dbo.usuario u
INNER JOIN dbo.privilegio p 
    ON u.id_privilegio = p.id_privilegio
WHERE u.correo = @correo";

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;

                    var param = cmd.CreateParameter();
                    param.ParameterName = "@correo";
                    param.Value = correo;
                    cmd.Parameters.Add(param);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                id_usuario = Convert.ToInt32(reader["id_usuario"]),
                                nombre = reader["nombre"]?.ToString(),
                                correo = reader["correo"]?.ToString(),
                                password = reader["password"]?.ToString(),
                                estado = Convert.ToInt32(reader["estado"]),
                                id_privilegio = Convert.ToInt32(reader["id_privilegio"]),
                                rolNombre = reader["rolNombre"]?.ToString()
                            };
                        }
                    }
                }
            }

            if (usuario == null) return null;

            if (!Crypter.CheckPassword(password, usuario.password)) return null;

            if (usuario.estado == 1) return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id_usuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.nombre ?? ""),
                new Claim(ClaimTypes.Email, usuario.correo ?? ""),
                new Claim(ClaimTypes.Role, usuario.rolNombre ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}