using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Reportes
{
    public class TblReporteUbicacionBE
    {
        public string Cod_Ubicacion { get; set; }
        public string Descripcion_Ubicacion { get; set; }
        public int Productos_Totales { get; set; }
        public decimal Stock_Inicial { get; set; }
        public int Productos_Contados { get; set; }
        public decimal Stock_Contado { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }
    }
}
