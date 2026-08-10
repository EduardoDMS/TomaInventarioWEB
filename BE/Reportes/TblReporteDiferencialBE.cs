namespace BE.Reportes
{
    public class TblReporteDiferencialBE
    {
        public string CodProducto { get; set; }
        public string DscProducto { get; set; }
        public decimal StockInicial { get; set; }
        // TODO: CAMBIAR NOMBRE A STOCK CONTADO
        public decimal StockFinal { get; set; }
        public decimal StockDiferencial { get; set; }
        public string Estado { get; set; }
        public string TipoDiferencia { get; set; }
    }
}
