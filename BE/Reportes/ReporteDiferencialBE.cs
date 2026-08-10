using System.Collections.Generic;

namespace BE.Reportes
{
    public class ReporteDiferencialBE
    {
        // KPIS
        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
        public int ProductosSinDiferencia { get; set; }
        public decimal TotalDeSobrantes { get; set; }
        public decimal TotalDeFaltantes { get; set; }
        public decimal DiferenciaNetaInventario { get; set; }
        public string EstadoInventario { get; set; }
        public int ConteoActual { get; set; }
        public int UltimoConteoFinalizado { get; set; }

        public int FilasTotal { get; set; }

        // TABLA
        public List<TblReporteDiferencialBE> TblReporteDiferencial { get; set; }
    }
}
