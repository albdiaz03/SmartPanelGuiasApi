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

        private IDbConnection GetConnection()
        {
            return _conexion.GetSmartPanelConnection();
        }

        private bool IsDevelopment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        }

        // ==============================
        // GET ALL
        // ==============================
        public List<Guia> GetAll()
        {
            var lista = new List<Guia>();

            using var conn = GetConnection();
            conn.Open();

            var query = IsDevelopment()
                ? "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen"
                : "SELECT \"NroInt\", \"Tipo\", \"Folio\", \"Fecha\", \"Descripcion\" FROM iw_gsaen";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;

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

            var query = IsDevelopment()
                ? "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen WHERE NroInt=@id"
                : "SELECT \"NroInt\", \"Tipo\", \"Folio\", \"Fecha\", \"Descripcion\" FROM iw_gsaen WHERE \"NroInt\"=@id";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;
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

            var maxQuery = IsDevelopment()
                ? "SELECT ISNULL(MAX(Folio), 0) + 1 FROM iw_gsaen WHERE Tipo=@tipo"
                : "SELECT COALESCE(MAX(\"Folio\"), 0) + 1 FROM iw_gsaen WHERE \"Tipo\"=@tipo";

            using var cmdFolio = conn.CreateCommand();
            cmdFolio.CommandText = maxQuery;
            cmdFolio.Parameters.Add(CreateParam(cmdFolio, "@tipo", g.Tipo));

            g.Folio = Convert.ToInt32(cmdFolio.ExecuteScalar());

            var insertQuery = IsDevelopment()
                ? "INSERT INTO iw_gsaen (Tipo, Folio, Fecha, Descripcion) VALUES (@tipo, @folio, @fecha, @desc)"
                : "INSERT INTO iw_gsaen (\"Tipo\", \"Folio\", \"Fecha\", \"Descripcion\") VALUES (@tipo, @folio, @fecha, @desc)";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = insertQuery;

            cmd.Parameters.Add(CreateParam(cmd, "@tipo", g.Tipo));
            cmd.Parameters.Add(CreateParam(cmd, "@folio", g.Folio));
            cmd.Parameters.Add(CreateParam(cmd, "@fecha", g.Fecha));
            cmd.Parameters.Add(CreateParam(cmd, "@desc", g.Descripcion));

            cmd.ExecuteNonQuery();
        }

        // ==============================
        // GET MAX FOLIO BY TIPO
        // ==============================
        public int GetMaxFolioByTipo(string tipo)
        {
            using var conn = GetConnection();
            conn.Open();

            var query = IsDevelopment()
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

            var updateQuery = IsDevelopment()
                ? "UPDATE iw_gsaen SET Tipo=@tipo, Folio=@folio, Fecha=@fecha, Descripcion=@desc WHERE NroInt=@id"
                : "UPDATE iw_gsaen SET \"Tipo\"=@tipo, \"Folio\"=@folio, \"Fecha\"=@fecha, \"Descripcion\"=@desc WHERE \"NroInt\"=@id";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = updateQuery;

            cmd.Parameters.Add(CreateParam(cmd, "@id", g.NroInt));
            cmd.Parameters.Add(CreateParam(cmd, "@tipo", g.Tipo));
            cmd.Parameters.Add(CreateParam(cmd, "@folio", g.Folio));
            cmd.Parameters.Add(CreateParam(cmd, "@fecha", g.Fecha));
            cmd.Parameters.Add(CreateParam(cmd, "@desc", g.Descripcion));

            cmd.ExecuteNonQuery();
        }

        // ==============================
        // DELETE
        // ==============================
        public void Delete(int id)
        {
            using var conn = GetConnection();
            conn.Open();

            var deleteQuery = IsDevelopment()
                ? "DELETE FROM iw_gsaen WHERE NroInt=@id"
                : "DELETE FROM iw_gsaen WHERE \"NroInt\"=@id";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = deleteQuery;
            cmd.Parameters.Add(CreateParam(cmd, "@id", id));

            cmd.ExecuteNonQuery();
        }

        // ==============================
        // HELPER
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