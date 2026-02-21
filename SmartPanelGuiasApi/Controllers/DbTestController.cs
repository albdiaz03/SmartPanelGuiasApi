using Microsoft.AspNetCore.Mvc;
using SmartPanelGuiasApi.Conexion;
using System.Collections.Generic;

namespace SmartPanelGuiasApi.Controllers
{
    [ApiController]
    [Route("api/testdb")]
    public class DbTestController : ControllerBase
    {
        private readonly DbConexion _db;

        public DbTestController(DbConexion db)
        {
            _db = db;
        }

        // Endpoint para listar todos los usuarios
        [HttpGet("usuarios")]
        public IActionResult GetUsuarios()
        {
            using var conn = _db.GetSmartPanelConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM usuario;";
            var reader = cmd.ExecuteReader();
            var list = new List<object>();
            while (reader.Read())
            {
                list.Add(new
                {
                    id = reader["id_usuario"],
                    nombre = reader["nombre"],
                    correo = reader["correo"]
                });
            }
            return Ok(list);
        }

        // Opcional: lista de privilegios
        [HttpGet("privilegios")]
        public IActionResult GetPrivilegios()
        {
            using var conn = _db.GetSmartPanelConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM privilegio;";
            var reader = cmd.ExecuteReader();
            var list = new List<object>();
            while (reader.Read())
            {
                list.Add(new
                {
                    id = reader["id_privilegio"],
                    nombre = reader["nombre_pri"],
                    descripcion = reader["descripcion"]
                });
            }
            return Ok(list);
        }
    }
}