using Microsoft.Data.SqlClient;

namespace SmartPanelGuiasApi.Conexion
{
    public class Conexion
    {
        private static string connectionString =
    "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSoftland;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
