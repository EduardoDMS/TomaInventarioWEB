using System.Collections.Generic;

namespace BE.Reportes
{
    public class ReporteUsuarioBE
    {
        // KPIS
        public int ProductosInventariados { get; set; }
        public int UsuariosParticipantes { get; set; }
        public int ProductosFueraInventario { get; set; }
        public string UsuarioMasLecturas { get; set; }
        public string UsuarioMenosLecturas { get; set; }
        public int ConteoSeleccionado { get; set; }
        public string EstadoConteo { get; set; }
        public int ConteosDisponibles { get; set; }


        // TABLA
        public List<TblReporteUsuarioBE> TblreporteUsuarios { get; set; }
    }
}
