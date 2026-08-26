namespace BE.Reportes
{
    public class TblReporteValorizadoBE
    {
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public string Ubicacion_Inicial { get; set; }
        public string Lote_Inicial { get; set; }
        public string Cod_Moneda { get; set; }
        public decimal Costo_Unitario { get; set; }
        public decimal Stock_Inicial { get; set; }
        public decimal Stock_Final { get; set; }
        public decimal Diferencia { get; set; }
        public decimal Impacto_Economico { get; set; }
        public string Observacion { get; set; }

    }

    public class FooterValorizadoBE
    {
        public decimal StockInicialTotal { get; set; }
        public decimal StockFinalTotal { get; set; }
        public decimal StockDiferencial { get; set; }
        public decimal ImpactoEconomicoTotal { get; set; }
    }
}
