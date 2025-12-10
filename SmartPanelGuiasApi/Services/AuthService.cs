namespace SmartPanelGuiasApi.Services
{
    public class AuthService
    {
        // Usuarios de prueba (lo puedes cambiar por DB después)
        private static readonly Dictionary<string, string> Usuarios = new()
        {
            { "admin", "1234" },
            { "beto", "5678" }
        };

        public bool ValidarUsuario(string usuario, string password)
        {
            return Usuarios.ContainsKey(usuario) && Usuarios[usuario] == password;
        }
    }
}
