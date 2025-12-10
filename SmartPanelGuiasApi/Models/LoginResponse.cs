namespace SmartPanelGuiasApi.Models
{
    public class LoginResponse
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; } = "";
        public string? Nombre { get; set; }
    }
}
