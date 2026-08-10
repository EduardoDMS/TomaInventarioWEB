namespace BE.Reportes
{
    public class TblReporteUsuarioBE
    {
        public string Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public int ProductosLecturados { get; set; }
        public int UbicacionesLecturadas { get; set; }
        public decimal StockTotalContado { get; set; }
        public string FechaHoraInicial { get; set; }
        public string FechaHoraFinal { get; set; }
        public string PromedioLecturas { get; set; }
        public string TiempoParticipacion { get; set; }
    }
}
