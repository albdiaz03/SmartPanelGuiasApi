using System.ComponentModel.DataAnnotations.Schema;

namespace SmartPanelGuiasApi.Models
{
    [Table("guias")]
    public class Guia
    {
        [Column("nroint")]
        public int NroInt { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } = "S";

        [Column("folio")]
        public int Folio { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; } = "";
    }
}