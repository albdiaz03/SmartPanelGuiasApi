namespace SmartPanelGuiasApi.Models
{
    public class Guia
    {
        public int NroInt { get; set; }
        public string Tipo { get; set; } = "S";
        public int Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
    }
}
