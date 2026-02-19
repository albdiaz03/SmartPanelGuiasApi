namespace SmartPanelGuiasApi.Services
{
    using Microsoft.Data.SqlClient;

    public class LogService
    {
        private readonly string _connectionString =
            "Server=ALBERTODIAZ\\SQLEXPRESS;Database=interfaceSmartPanel;User Id=sa;Password=admin;Encrypt=True;TrustServerCertificate=True;";

        public async Task RegistrarLog(int idUsuario, string accion, string descripcion, string ip, string navegador)
        {
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            var query = @"INSERT INTO Logs (IdUsuario, Accion, Descripcion, IP, Navegador)
                      VALUES (@IdUsuario, @Accion, @Descripcion, @IP, @Navegador)";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@Accion", accion);
            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
            cmd.Parameters.AddWithValue("@IP", ip);
            cmd.Parameters.AddWithValue("@Navegador", navegador);

            await cmd.ExecuteNonQueryAsync();
        }
    }

}
