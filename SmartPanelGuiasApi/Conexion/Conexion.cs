using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace SmartPanelGuiasApi.Conexion
{
    public class DatabaseConnection
    {
        private readonly string connectionSmartPanelLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSmartPanel;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        private readonly string connectionSoftlandLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=InterfaceSoftland;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        public IDbConnection GetSmartPanelConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env == "Development")
            {
                return new SqlConnection(connectionSmartPanelLocal);
            }
            else
            {
                var connectionString =
                    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

                return new NpgsqlConnection(connectionString);
            }
        }

        public IDbConnection GetSoftlandConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (env == "Development")
            {
                return new SqlConnection(connectionSoftlandLocal);
            }
            else
            {
                var connectionString =
                    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

                return new NpgsqlConnection(connectionString);
            }
        }
    }
}