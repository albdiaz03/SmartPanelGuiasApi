using Microsoft.Data.SqlClient;

namespace SmartPanelGuiasApi.Conexion
{
    public class Conexion
    {
        private static string connectionSmartPanel =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSmartPanel;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        private static string connectionSoftland =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=InterfaceSoftland;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        public static SqlConnection GetSmartPanelConnection()
        {
            return new SqlConnection(connectionSmartPanel);
        }

        public static SqlConnection GetSoftlandConnection()
        {
            return new SqlConnection(connectionSoftland);
        }
    }
}
