namespace SmartPanelGuiasApi.Dtos
{
    public class GuiaCreateDto
    {
        public string Tipo { get; set; }
        public int Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}
