using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Reportes
{
    public class ReporteValorizadoBE
    {
        public decimal Valorizado_Total_Inventariado { get; set; }
        public decimal Valor_final_Inventariado {get; set;}
        public decimal Impacto_Economico_Neto { get; set; }
        public decimal Valorizado_Sobrantes { get; set; }
        public decimal Valorizado_Faltantes { get; set; }

        public int Cantidad_Productos_Impacto { get; set; }
        public int Cantidad_Productos_Sin_Costo { get; set; }
        public string Codigo_Inventario { get; set; } = string.Empty;
        public string Estado_Inventario { get; set; } = string.Empty;

        public List<TblReporteValorizadoBE> tblReporteValorizadoBEs { get; set; }
        public FooterValorizadoBE footer { get; set; }

    }
}
