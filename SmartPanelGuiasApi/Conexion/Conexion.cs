using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace SmartPanelGuiasApi.Conexion
{
    public class DbConexion
    {
        // 🔹 Conexiones locales SQL Server
        private readonly string connectionSmartPanelLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSmartPanel;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        private readonly string connectionSoftlandLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=InterfaceSoftland;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        // 🔹 Obtiene conexión SmartPanel
        public IDbConnection GetSmartPanelConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env == "Development")
            {
                return new SqlConnection(connectionSmartPanelLocal);
            }
            else
            {
                var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
                if (string.IsNullOrEmpty(connStr))
                    throw new InvalidOperationException("ConnectionStrings__DefaultConnection no está configurada.");

                return new NpgsqlConnection(connStr); // ✅ Usar connection string directo
            }
        }

        // 🔹 Obtiene conexión Softland
        public IDbConnection GetSoftlandConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env == "Development")
            {
                return new SqlConnection(connectionSoftlandLocal);
            }
            else
            {
                var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
                if (string.IsNullOrEmpty(connStr))
                    throw new InvalidOperationException("ConnectionStrings__DefaultConnection no está configurada.");

                return new NpgsqlConnection(connStr); // ✅ Usar connection string directo
            }
        }
    }
}