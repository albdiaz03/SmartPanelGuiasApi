using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace SmartPanelGuiasApi.Conexion
{
    public class DbConexion
    {
        private readonly string connectionSmartPanelLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSmartPanel;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        private readonly string connectionSoftlandLocal =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=InterfaceSoftland;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        public IDbConnection GetSmartPanelConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development") return new SqlConnection(connectionSmartPanelLocal);

            var url = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = uri.AbsolutePath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return new NpgsqlConnection(builder.ConnectionString);
        }

        public IDbConnection GetSoftlandConnection()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development") return new SqlConnection(connectionSoftlandLocal);

            var url = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = uri.AbsolutePath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            return new NpgsqlConnection(builder.ConnectionString);
        }
    }
}