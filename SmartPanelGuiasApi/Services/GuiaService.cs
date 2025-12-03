using Microsoft.Data.SqlClient;
using SmartPanelGuiasApi.Models;
using System;
using System.Collections.Generic;

namespace SmartPanelGuiasApi.Services
{
    public class GuiaService
    {
        public List<Guia> GetAll()
        {
            var lista = new List<Guia>();

            using (var conn = SmartPanelGuiasApi.Conexion.Conexion.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Guia
                        {
                            NroInt = reader.GetInt32(0),
                            Tipo = reader.GetString(1),
                            Folio = reader.GetInt32(2),
                            Fecha = reader.GetDateTime(3),
                            Descripcion = reader.GetString(4)
                        });
                    }
                }
            }

            return lista;
        }

        public Guia Get(int id)
        {
            Guia guia = null;

            using (var conn = Conexion.Conexion.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "SELECT NroInt, Tipo, Folio, Fecha, Descripcion FROM iw_gsaen WHERE NroInt=@id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        guia = new Guia
                        {
                            NroInt = reader.GetInt32(0),
                            Tipo = reader.GetString(1),
                            Folio = reader.GetInt32(2),
                            Fecha = reader.GetDateTime(3),
                            Descripcion = reader.GetString(4)
                        };
                    }
                }
            }

            return guia;
        }

        public void Create(Guia g)
        {
            using (var conn = Conexion.Conexion.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "INSERT INTO iw_gsaen (Tipo, Folio, Fecha, Descripcion) VALUES (@tipo, @folio, @fecha, @desc)",
                    conn
                );

                cmd.Parameters.AddWithValue("@tipo", g.Tipo);
                cmd.Parameters.AddWithValue("@folio", g.Folio);
                cmd.Parameters.AddWithValue("@fecha", g.Fecha);
                cmd.Parameters.AddWithValue("@desc", g.Descripcion);

                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Guia g)
        {
            using (var conn = Conexion.Conexion.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "UPDATE iw_gsaen SET Tipo=@tipo, Folio=@folio, Fecha=@fecha, Descripcion=@desc WHERE NroInt=@id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", g.NroInt);
                cmd.Parameters.AddWithValue("@tipo", g.Tipo);
                cmd.Parameters.AddWithValue("@folio", g.Folio);
                cmd.Parameters.AddWithValue("@fecha", g.Fecha);
                cmd.Parameters.AddWithValue("@desc", g.Descripcion);

                cmd.ExecuteNonQuery();
            }
        }


        public void Delete(int id)
        {
            using (var conn = Conexion.Conexion.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "DELETE FROM iw_gsaen WHERE NroInt=@id",
                    conn
                );

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
