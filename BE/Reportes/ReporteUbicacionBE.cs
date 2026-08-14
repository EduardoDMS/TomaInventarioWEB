using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BE.Reportes
{
    public class ReporteUbicacionBE
    {
        public int Ubicaciones_Totales { get; set; }
        public int Ubicaciones_Nuevas { get; set; }
        public int Ubicaciones_Completas { get; set; }
        public int Ubicaciones_En_Proceso { get; set; }
        public int Ubicaciones_No_Iniciadas { get; set; }
        public string Estado_Inventario { get; set; }
        public string Cod_Inventario { get; set; }
        public int Conteo_Actual { get; set; }
        public int Ultimo_Conteo_Finalizado { get; set; }
        public string Top3_Mayor_Diferencia { get; set; }
        public string Top3_Menor_Diferencia { get; set; }

        public List<TblReporteUbicacionBE> TblReporteUbicacion { get; set; }
    }
}
