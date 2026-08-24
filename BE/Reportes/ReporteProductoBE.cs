using System.Collections.Generic;

namespace BE.Reportes
{
    public class ReporteProductoBE
    {
        // KPIS
        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
        public int ProductosSinDiferencia { get; set; }
        public int ProductosNoLecturados { get; set; }
        public int ProductosUbicacionDiferente { get; set; }
        public int ProductosLoteDiferente { get; set; }
        public decimal StockInicialTotal { get; set; }
        public decimal StockFinalTotal { get; set; }
        public decimal DiferenciaTotal { get; set; }
        public string EstadoConteo { get; set; }
        public int ConteoSeleccionado { get; set; }

        // TABLA
        public List<TblReporteProductoBE> TblReporteProductos { get; set; }
    }
}
