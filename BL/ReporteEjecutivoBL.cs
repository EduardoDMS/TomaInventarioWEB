using DAO.ReporteResumenEjecutivo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;


namespace BL
{
    public class ReporteEjecutivoViewModel
    {

        // NUEVO AÑADIDO
        public string CodInventario { get; set; }
        public string CodAlmacen { get; set; }
        public string DscAlmacen { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierreFinal { get; set; }
        // public int ConteosRealizados { get; set; }
        public int DuracionHoras { get; set; }
        public int ConteosRealizados { get; set; }
        public int UsuariosParticipantes { get; set; }

        // KPI
        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
        public int ProductosFaltantes { get; set; }
        public int ProductosSobrantes { get; set; }
        public int ProductosFueraUbicacion { get; set; }

        // stock
        public decimal SumaSobrantes { get; set; }
        public decimal SumaFaltantes { get; set; }

        public decimal DiferenciaNeta { get; set; }

        public List<ProductoDiferenciaDto> Top10Productos { get; set; } = new List<ProductoDiferenciaDto>();
        public List<UbicacionDiferenciaDto> Top10Ubicaciones { get; set; } = new List<UbicacionDiferenciaDto>();
        public List<ProductoFueraUbicacionDto> Top10ProductosFueraUbicacion { get; set; } = new List<ProductoFueraUbicacionDto>();
        public List<UsuarioParticipacionDto> Usuarios { get; set; } = new List<UsuarioParticipacionDto>();

        public decimal ExactitudPct =>
        ProductosInventariados == 0
            ? 0
            : Math.Round(
                (ProductosInventariados - ProductosConDiferencia)
                * 100m
                / ProductosInventariados,
                2
            );


        public string ResumenEjecutivo { get; set; }
    }

    public class ReporteEjecutivoBL
    {
        private readonly ReporteEjecutivoDAO _dao;

        public ReporteEjecutivoBL()
        {
            _dao = new ReporteEjecutivoDAO();
        }

        public ReporteEjecutivoViewModel ObtenerReporte(string codInventario)
        {
            Response responseInfo = _dao.ObtenerInfo(codInventario);

            if (responseInfo.HUBO_ERROR)
                throw new Exception(responseInfo.MENSAJE_ERROR);

            InfoInventarioDao info = responseInfo.Entity as InfoInventarioDao;

            if (info == null)
                throw new InvalidOperationException(
                    "No existe el inventario " + codInventario);

            Response responseKpi = _dao.ObtenerProductosKpi(codInventario);

            if (responseKpi.HUBO_ERROR)
                throw new Exception(responseKpi.MENSAJE_ERROR);

            ProductosKpiDao kpi = responseKpi.Entity as ProductosKpiDao;

            Response responseDiferencias = _dao.ObtenerDiferencias(codInventario);

            if (responseDiferencias.HUBO_ERROR)
                throw new Exception(
                    responseDiferencias.MENSAJE_ERROR);

            DiferenciasResultDao diferencias = responseDiferencias.Entity as DiferenciasResultDao;

            Response responseUbicaciones = _dao.ObtenerUbicacionesConDiferencia(codInventario);

            if (responseUbicaciones.HUBO_ERROR)
                throw new Exception(
                    responseUbicaciones.MENSAJE_ERROR);

            UbicacionesResultDao ubicaciones = responseUbicaciones.Entity as UbicacionesResultDao;

            Response responseFueraUbicacion = _dao.ObtenerFueraUbicacion(codInventario);

            if (responseFueraUbicacion.HUBO_ERROR)
                throw new Exception(
                    responseFueraUbicacion.MENSAJE_ERROR);

            FueraUbicacionResultDao fueraUbicacion = responseFueraUbicacion.Entity as FueraUbicacionResultDao;

            Response responseUsuarios = _dao.ObtenerUsuarios(codInventario);

            if (responseUsuarios.HUBO_ERROR)
                throw new Exception(
                    responseUsuarios.MENSAJE_ERROR);

            UsuariosResultDao usuarios = responseUsuarios.Entity as UsuariosResultDao;

            ReporteEjecutivoViewModel vm = new ReporteEjecutivoViewModel();

            // Información general
            vm.CodInventario = codInventario;
            vm.CodAlmacen = info.CodAlmacen;
            vm.DscAlmacen = info.DscAlmacen;
            vm.FechaInicio = info.FechaInicio;
            vm.FechaCierreFinal = info.FechaCierreFinal;
            vm.DuracionHoras = info.DuracionHoras;
            vm.ConteosRealizados = info.ConteosRealizados;
            vm.ProductosInventariados = kpi.ProductosInventariados; 
            vm.ProductosConDiferencia = kpi.ProductosConDiferencia; 
            vm.ProductosFaltantes = kpi.ProductosFaltantes; 
            vm.ProductosSobrantes = kpi.ProductosSobrantes; 

            // Stock
            vm.SumaSobrantes = diferencias.SumaSobrantes;
            vm.SumaFaltantes = diferencias.SumaFaltantes;
            vm.DiferenciaNeta = diferencias.DiferenciaNeta;
            vm.Top10Productos = diferencias.Top10;
            vm.Top10Ubicaciones = ubicaciones.Top10;
            vm.ProductosFueraUbicacion = fueraUbicacion.Total;
            vm.Top10ProductosFueraUbicacion = fueraUbicacion.Productos;
            vm.UsuariosParticipantes = usuarios.UsuariosParticipantes;
            vm.Usuarios = usuarios.Usuarios;

            // Resumen
            vm.ResumenEjecutivo = ArmarResumenTexto(vm);

            return vm;
        }

        private string ArmarResumenTexto(ReporteEjecutivoViewModel vm)
        {
            var topProducto = vm.Top10Productos.FirstOrDefault();
            var sb = new StringBuilder();

            sb.Append($"El inventario registró {vm.ProductosInventariados:N0} productos inventariados, ");
            sb.Append($"con una exactitud global del {vm.ExactitudPct:0.00}%. ");
            sb.Append($"Se identificaron {vm.ProductosFaltantes:N0} productos faltantes ");
            sb.Append($"y {vm.ProductosSobrantes:N0} productos sobrantes. ");
            sb.Append($"El stock presentó {Math.Abs(vm.SumaFaltantes):N0} unidades faltantes ");
            sb.Append($"y {vm.SumaSobrantes:N0} unidades sobrantes. ");
            sb.Append($"Se detectaron {vm.ProductosFueraUbicacion:N0} productos " + "fuera de su ubicación registrada. ");
            sb.Append($"Participaron {vm.UsuariosParticipantes:N0} usuarios.");

            if (topProducto != null)
            {
                sb.Append(
                    $" El producto con mayor diferencia fue "
                    + $"\"{topProducto.DscProducto}\" "
                    + $"con {topProducto.Diferencia:+#,##0;-#,##0;0} unidades."
                );
            }

            return sb.ToString();
        }
    }
}
