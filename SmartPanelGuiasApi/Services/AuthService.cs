using Microsoft.Data.SqlClient;
using SmartPanelGuiasApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartPanelGuiasApi.Conexion;   // 🔥 ESTE using ES CLAVE
using Microsoft.Data.SqlClient;
using CryptSharp;



namespace SmartPanelGuiasApi.Services
{
    public class AuthService
    {
        private readonly string _key = "ESTA_ES_MI_CLAVE_SUPER_SECRETA_2026";

        public string Login(string correo, string password)
        {
            Usuario usuario = null;

            using (SqlConnection conn = SmartPanelGuiasApi.Conexion.Conexion.GetSmartPanelConnection())

            {
                conn.Open();

                string query = @"
SELECT u.*, p.nombre_pri AS rolNombre
FROM dbo.usuario u
INNER JOIN dbo.privilegio p 
    ON u.id_privilegio = p.id_privilegio
WHERE u.correo = @correo";



                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@correo", correo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
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

            // 🔴 No existe usuario
            if (usuario == null)
                return null;

            // 🔐 Validar contraseña
            if (!Crypter.CheckPassword(password, usuario.password))
                return null;

            // 🚦 Validar estado
            if (usuario.estado == 1)
                return null;

            // 🎟 Crear JWT

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
