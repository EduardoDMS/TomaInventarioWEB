using System.Collections.Generic;

namespace BE.Reportes
{
    public class ReporteAuditoriaBE
    {
        public int Productos_Inventariados { get; set; }
        public int Productos_Con_Diferencia { get; set; }
        public int Productos_Sin_Diferencia { get; set; }
        public int Usuarios_Participantes { get; set; }
        public string Primera_Lectura { get; set; }
        public string Ultima_Lectura { get; set; }
        public string TiempoTotalInventario { get; set; }
        public string Estado_Inventario { get; set; }
        public string Codigo_Inventario { get; set; }
        public int Conteo_Actual { get; set; }

        public List<TblReporteAuditoriaBE> TblReporteAuditoria { get; set; }

        // agregado p 
        public FooterAuditoriaBE Footer { get; set; }

    }
}
