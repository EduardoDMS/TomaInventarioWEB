using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Reportes
{
    public class TblReporteAuditoriaBE
    {
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public decimal Stock_Inicial { get; set; }
        public string Ubicacion_Inicial { get; set; }
        public string Lote_Inicial { get; set; }
        public decimal Stock_Final { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }


        public List<ConteoDetalleBE> Conteos { get; set; }
    }



    // la parte del conteoDetalle es para la tabla sql que es "REACTIVA" , si cierras desde el conteo 1 solo se queda en conteo 1 sin invocar el conteo 2 o 3 
    // logica en sql hay que recordar de validar todo lo necesario ya sea 
    public class ConteoDetalleBE
    {
        public int Nro_Conteo { get; set; }// servira para darnos el numero de conteo 1,2,3 y colocarlo en los campos de la tabla a armar
        public decimal? Stock_Contado { get; set; } //puede venir como nulo por que no se ha contado todavia
        public string Ubicaciones_Contadas { get; set; }
        public string Lote_Contado { get; set; }
        public string Usuario { get; set; }
        public string Hora_Registro { get; set; }
    }

    // A partir de aqui se hara el cambio grotesco se recomienda discrecion

    public class ConteoTotalBE
    {
        public int Nro_ConteoTotal { get; set; }
        public decimal? Total_Stock { get; set; }
    }

    public class FooterAuditoriaBE
    {
        public decimal StockInicialTotal { get; set; }
        public List<ConteoTotalBE> TotalesPorConteo { get; set; }
        public decimal StockFinalTotal { get; set; }
        public decimal StockDiferencial {  get; set; }
    }
}
