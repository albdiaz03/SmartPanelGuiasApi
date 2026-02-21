using SmartPanelGuiasApi.Models;
using SmartPanelGuiasApi.Conexion;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmartPanelGuiasApi.Services
{
    public class GuiaService
    {
        private readonly DbConexion _conexion;

        public GuiaService(DbConexion conexion)
        {
            _conexion = conexion;
        }

        // 🔹 Método que elige la conexión según el entorno
        private IDbConnection GetConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return _conexion.GetSmartPanelConnection(); // SmartPanel: local SQL Server / nube PostgreSQL
        }

        // ==============================
        // GET ALL
        // ==============================
        public List<Guia> GetAll()
        {
            var lista = new List<Guia>();

            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Guia
                {
                    NroInt = Convert.ToInt32(reader["NroInt"]),
                    Tipo = reader["Tipo"]?.ToString(),
                    Folio = Convert.ToInt32(reader["Folio"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Descripcion = reader["Descripcion"]?.ToString()
                });
            }

            return lista;
        }

        // ==============================
        // GET BY ID
        // ==============================
        public Guia Get(int id)
        {
            Guia guia = null;

            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen WHERE NroInt=@id";
            cmd.Parameters.Add(CreateParam(cmd, "@id", id));

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                guia = new Guia
                {
                    NroInt = Convert.ToInt32(reader["NroInt"]),
                    Tipo = reader["Tipo"]?.ToString(),
                    Folio = Convert.ToInt32(reader["Folio"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Descripcion = reader["Descripcion"]?.ToString()
                };
            }

            return guia;
        }

        // ==============================
        // CREATE
        // ==============================
        public void Create(Guia g)
        {
            using var conn = GetConnection();
            conn.Open();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var maxQuery = env == "Development"
                ? "SELECT ISNULL(MAX(Folio), 0) + 1 FROM iw_gsaen WHERE Tipo=@tipo"
                : "SELECT COALESCE(MAX(\"Folio\"), 0) + 1 FROM iw_gsaen WHERE \"Tipo\"=@tipo";

            using var cmdFolio = conn.CreateCommand();
            cmdFolio.CommandText = maxQuery;
            cmdFolio.Parameters.Add(CreateParam(cmdFolio, "@tipo", g.Tipo));
            g.Folio = Convert.ToInt32(cmdFolio.ExecuteScalar());

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO iw_gsaen (Tipo, Folio, Fecha, Descripcion) VALUES (@tipo, @folio, @fecha, @desc)";
            cmd.Parameters.AddRange(new[]
            {
                CreateParam(cmd, "@tipo", g.Tipo),
                CreateParam(cmd, "@folio", g.Folio),
                CreateParam(cmd, "@fecha", g.Fecha),
                CreateParam(cmd, "@desc", g.Descripcion)
            });
            cmd.ExecuteNonQuery();
        }

        // ==============================
        // GET MAX FOLIO BY TIPO
        // ==============================
        public int GetMaxFolioByTipo(string tipo)
        {
            using var conn = GetConnection();
            conn.Open();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var query = env == "Development"
                ? "SELECT ISNULL(MAX(Folio), 0) FROM iw_gsaen WHERE Tipo=@tipo"
                : "SELECT COALESCE(MAX(\"Folio\"), 0) FROM iw_gsaen WHERE \"Tipo\"=@tipo";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.Add(CreateParam(cmd, "@tipo", tipo));

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // ==============================
        // UPDATE
        // ==============================
        public void Update(Guia g)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE iw_gsaen SET Tipo=@tipo, Folio=@folio, Fecha=@fecha, Descripcion=@desc WHERE NroInt=@id";
            cmd.Parameters.AddRange(new[]
            {
                CreateParam(cmd, "@id", g.NroInt),
                CreateParam(cmd, "@tipo", g.Tipo),
                CreateParam(cmd, "@folio", g.Folio),
                CreateParam(cmd, "@fecha", g.Fecha),
                CreateParam(cmd, "@desc", g.Descripcion)
            });

            cmd.ExecuteNonQuery();
        }

        // ==============================
        // DELETE
        // ==============================
        public void Delete(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM iw_gsaen WHERE NroInt=@id";
            cmd.Parameters.Add(CreateParam(cmd, "@id", id));

            cmd.ExecuteNonQuery();
        }

        // ==============================
        // HELPER: crear parámetros
        // ==============================
        private IDbDataParameter CreateParam(IDbCommand cmd, string name, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }
    }
}