using System.Collections.Generic;

namespace BE.Reportes
{
    public class ReporteConteoBE
    {
        // KPIS
        public int ProductosInventariados { get; set; }
        public int ConteoSeleccionado { get; set; }
        public int LecturasRealizadas { get; set; }
        public int UsuariosParticipantes { get; set; }
        public int ProductosFueraInventario { get; set; }
        public string HoraPrimeraLectura { get; set; }
        public string HoraUltimaLectura { get; set; }
        public string DuracionConteo { get; set; }
        public string EstadoConteo { get; set; }
        public int ConteosDisponibles { get; set; }

        // TABLA
        public List<TblReporteConteoBE> TblReporteConteo { get; set; }
    }
}
