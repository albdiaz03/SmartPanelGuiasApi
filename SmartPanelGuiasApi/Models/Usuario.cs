namespace SmartPanelGuiasApi.Models
{
    public class Usuario
    {
        public int id_usuario { get; set; }
        public int id_privilegio { get; set; }
        public string rut { get; set; }
        public string password { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public int estado { get; set; }
    }

}
