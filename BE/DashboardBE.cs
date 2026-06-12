using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    /// <summary>
    /// Entidad para los KPIs principales del dashboard
    /// </summary>
    public class DashboardKPIsBE
    {
        public int TotalAlmacenes { get; set; }
        public int TotalInventarios { get; set; }
        public int TotalProductos { get; set; }
    }

    /// <summary>
    /// Entidad para el gráfico de inventarios por mes
    /// </summary>
    public class DashboardInventariosMesBE
    {
        public int NumMes { get; set; }
        public string NombreMes { get; set; }
        public int CantidadInventarios { get; set; }
    }

    /// <summary>
    /// Últimos inventarios
    /// </summary>
    public class DashboardUltimosInventariosBE
    {
        public int ID_INVENTARIO { get; set; }
        public string COD_INVENTARIO { get; set; }
        public string NombreAlmacen { get; set; }
        public int NRO_CONTEO { get; set; }
        public string EtiquetaConteo { get; set; }
        public string FechaFormato { get; set; }
        public DateTime FCH_INICIO { get; set; }
        public DateTime? FCH_FIN { get; set; }
        public string COD_ESTADO { get; set; }
        public string DescripcionEstado { get; set; }
        public string ColorConteo { get; set; }
        public string ColorEstado { get; set; }
    }

    /// <summary>
    /// Estadísticas adicionales
    /// </summary>
    public class DashboardEstadisticasBE
    {
        public int TotalInventariosAnio { get; set; }
        public decimal PromedioMensual { get; set; }
        public int InventariosAbiertos { get; set; }
        public int InventariosCerrados { get; set; }
    }

    /// <summary>
    /// Entidad principal que agrupa todos los datos del dashboard
    /// </summary>
    public class DashboardCompletoBE
    {
        public DashboardKPIsBE KPIs { get; set; }
        public List<DashboardInventariosMesBE> InventariosPorMes { get; set; }
        public List<DashboardUltimosInventariosBE> UltimosInventarios { get; set; }
        public DashboardEstadisticasBE Estadisticas { get; set; }

        public DashboardCompletoBE()
        {
            KPIs = new DashboardKPIsBE();
            InventariosPorMes = new List<DashboardInventariosMesBE>();
            UltimosInventarios = new List<DashboardUltimosInventariosBE>();
            Estadisticas = new DashboardEstadisticasBE();
        }
    }
}