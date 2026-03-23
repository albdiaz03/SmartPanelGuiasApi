using CryptSharp;
using SmartPanelGuiasApi.Conexion;
using SmartPanelGuiasApi.Helpers;
using SmartPanelGuiasApi.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace SmartPanelGuiasApi.Services
{
    public class AuthService
    {
        private readonly DbConexion _db;

        public AuthService(DbConexion db)
        {
            _db = db;
        }

        public string? Login(string correo, string password)
        {
            using var conn = _db.GetSmartPanelConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id_usuario, password FROM usuario WHERE correo = @correo";
            var param = cmd.CreateParameter();
            param.ParameterName = "@correo";
            param.Value = correo;
            cmd.Parameters.Add(param);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            string hashedPassword = reader.GetString(1);
            bool valid = Crypter.CheckPassword(password, hashedPassword);
            if (!valid) return null;

            // Aquí generas tu token JWT (puedes usar tu código actual)
            return JwtHelper.GenerateToken(correo);
        }
    }
}