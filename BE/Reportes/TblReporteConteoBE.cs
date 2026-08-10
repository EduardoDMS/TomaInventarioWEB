namespace BE.Reportes
{
    public class TblReporteConteoBE
    {
        public string CodProducto { get; set; }
        public string DscProducto { get; set; }
        public string UbicacionInicial { get; set; }
        public string UbicacionContada { get; set; }
        public string LoteInicial { get; set; }
        public string LoteContado { get; set; }
        public decimal StockInicial { get; set; }
        public decimal StockContado { get; set; }
        public decimal StockDiferencial { get; set; }
        public string Usuario { get; set; }
    }
}
