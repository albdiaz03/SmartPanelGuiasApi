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


        public List<Guia> GetAll()
        {
            var lista = new List<Guia>();

            using (var conn = _conexion.GetSoftlandConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen";

                    using (var reader = cmd.ExecuteReader())
                    {
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
                    }
                }
            }

            return lista;
        }

        public Guia Get(int id)
        {
            Guia guia = null;

            using (var conn = _conexion.GetSoftlandConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen WHERE NroInt=@id";

                    var param = cmd.CreateParameter();
                    param.ParameterName = "@id";
                    param.Value = id;
                    cmd.Parameters.Add(param);

                    using (var reader = cmd.ExecuteReader())
                    {
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
                    }
                }
            }

            return guia;
        }

        public void Create(Guia g)
        {
            using (var conn = _conexion.GetSoftlandConnection())
            {
                conn.Open();

                // IMPORTANTE:
                // PostgreSQL usa COALESCE, SQL Server usa ISNULL
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var maxQuery = env == "Development"
                    ? "SELECT ISNULL(MAX(Folio), 0) + 1 FROM iw_gsaen WHERE Tipo=@tipo"
                    : "SELECT COALESCE(MAX(\"Folio\"), 0) + 1 FROM iw_gsaen WHERE \"Tipo\"=@tipo";

                using (var cmdFolio = conn.CreateCommand())
                {
                    cmdFolio.CommandText = maxQuery;

                    var paramTipo = cmdFolio.CreateParameter();
                    paramTipo.ParameterName = "@tipo";
                    paramTipo.Value = g.Tipo;
                    cmdFolio.Parameters.Add(paramTipo);

                    g.Folio = Convert.ToInt32(cmdFolio.ExecuteScalar());
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO iw_gsaen (Tipo, Folio, Fecha, Descripcion) VALUES (@tipo, @folio, @fecha, @desc)";

                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@tipo";
                    p1.Value = g.Tipo;
                    cmd.Parameters.Add(p1);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@folio";
                    p2.Value = g.Folio;
                    cmd.Parameters.Add(p2);

                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@fecha";
                    p3.Value = g.Fecha;
                    cmd.Parameters.Add(p3);

                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@desc";
                    p4.Value = g.Descripcion;
                    cmd.Parameters.Add(p4);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetMaxFolioByTipo(string tipo)
        {
            using var conn = _conexion.GetSoftlandConnection();
            conn.Open();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var query = env == "Development"
                ? "SELECT ISNULL(MAX(Folio), 0) FROM iw_gsaen WHERE Tipo=@tipo"
                : "SELECT COALESCE(MAX(\"Folio\"), 0) FROM iw_gsaen WHERE \"Tipo\"=@tipo";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = query;

            var param = cmd.CreateParameter();
            param.ParameterName = "@tipo";
            param.Value = tipo;
            cmd.Parameters.Add(param);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Guia g)
        {
            using (var conn = _conexion.GetSoftlandConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE iw_gsaen SET Tipo=@tipo, Folio=@folio, Fecha=@fecha, Descripcion=@desc WHERE NroInt=@id";

                    var p1 = cmd.CreateParameter();
                    p1.ParameterName = "@id";
                    p1.Value = g.NroInt;
                    cmd.Parameters.Add(p1);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "@tipo";
                    p2.Value = g.Tipo;
                    cmd.Parameters.Add(p2);

                    var p3 = cmd.CreateParameter();
                    p3.ParameterName = "@folio";
                    p3.Value = g.Folio;
                    cmd.Parameters.Add(p3);

                    var p4 = cmd.CreateParameter();
                    p4.ParameterName = "@fecha";
                    p4.Value = g.Fecha;
                    cmd.Parameters.Add(p4);

                    var p5 = cmd.CreateParameter();
                    p5.ParameterName = "@desc";
                    p5.Value = g.Descripcion;
                    cmd.Parameters.Add(p5);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _conexion.GetSoftlandConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM iw_gsaen WHERE NroInt=@id";

                    var param = cmd.CreateParameter();
                    param.ParameterName = "@id";
                    param.Value = id;
                    cmd.Parameters.Add(param);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}