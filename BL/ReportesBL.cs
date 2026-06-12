using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ReportesBL
    {
        public Response TablaHistorico(int ID_Almacen, string filtrarFecha, string dtmIni, string dtmFin)
        {
            return new ReportesDAO().TablaHistorico(ID_Almacen, filtrarFecha, dtmIni, dtmFin);
        }
        public Response TablaDetalle_WEB(string cod_Inventario, int NConteo, string start, string length, string order, string search)
        {
            return new ReportesDAO().TablaDetalle_WEB(cod_Inventario, NConteo, start, length, order, search);
        }

        public Response TablaDetalle(string cod_Inventario, int NConteo)
        {
            return new ReportesDAO().TablaDetalle(cod_Inventario, NConteo);
        }

        public Response TablaLecturas(string cod_Inventario, int NConteo)
        {
            return new ReportesDAO().TablaLecturas(cod_Inventario, NConteo);
        }
    }
}
