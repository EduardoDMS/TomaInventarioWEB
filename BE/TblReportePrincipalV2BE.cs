using System;

namespace BE
{
    public class TblReportePrincipalV2BE
    {
        // Datos del producto
        public string Cod_Producto { get; set; }
        public string Dsc_Producto { get; set; }
        public string Cod_ubicacion { get; set; }
        public string Lote_Producto { get; set; }
        public string Serie_Producto { get; set; }

        // Stocks y conteos (NUEVO: solo último conteo finalizado)
        public decimal Stock_inicial { get; set; }
        public int Ultimo_Conteo_Finalizado { get; set; }
        public decimal Stock_Ultimo_Conteo { get; set; }
        public decimal Stock_Diferencial { get; set; }

        // Estado y análisis
        public string Estado_Ajuste { get; set; }
        public string DSC_ANALISIS { get; set; }
        public string DSC_OBSERVACION { get; set; }

        // Usuario y fechas
        public string Cod_Usuario_Registro { get; set; }
        public string Fch_Registro_Inventario { get; set; }
        public string Fch_Cierre { get; set; }

        // Estado del inventario (NUEVO)
        public string Estado_Inventario { get; set; }
        public int Conteo_Actual { get; set; }
        public int ESTADO_CONTEO { get; set; }
        public int FCH_REGISTRO_INVENTARIO { get; set; }

        // Constructor
        //public TblReportePrincipalV2BE()
        //{
        //    Cod_Producto = string.Empty;
        //    Dsc_Producto = string.Empty;
        //    Cod_ubicacion = string.Empty;
        //    Lote_Producto = string.Empty;
        //    Serie_Producto = string.Empty;
        //    Stock_inicial = 0;
        //    Ultimo_Conteo_Finalizado = 0;
        //    Stock_Ultimo_Conteo = 0;
        //    Stock_Diferencial = 0;
        //    Estado_Ajuste = string.Empty;
        //    DSC_ANALISIS = string.Empty;
        //    DSC_OBSERVACION = string.Empty;
        //    Cod_Usuario_Registro = string.Empty;
        //    Fch_Registro_Inventario = string.Empty;
        //    Fch_Cierre = string.Empty;
        //    Estado_Inventario = string.Empty;
        //    Conteo_Actual = 0;
        //}
    }

}