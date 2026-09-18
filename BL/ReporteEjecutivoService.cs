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
        public string CodInventario { get; set; }
        public string CodAlmacen { get; set; }
        public string DscAlmacen { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierreFinal { get; set; }
        public int DuracionDias { get; set; }
        public string Estado { get; set; }
        public int ConteosRealizados { get; set; }

        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
        public int UsuariosParticipantes { get; set; }
        public int UbicacionesConDiferencias { get; set; }
        public decimal SumaSobrantes { get; set; }
        public decimal SumaFaltantes { get; set; }
        public decimal DiferenciaNeta { get; set; }

        public decimal ExactitudPct => ProductosInventariados == 0
            ? 0
            : Math.Round((ProductosInventariados - ProductosConDiferencia) * 100m / ProductosInventariados, 2);

        public List<ProductoDiferenciaDto> Top10Productos { get; set; } = new List<ProductoDiferenciaDto>();
        public List<UbicacionDiferenciaDto> Top10Ubicaciones { get; set; } = new List<UbicacionDiferenciaDto>();
        public List<UsuarioParticipacionDto> TopUsuarios { get; set; } = new List<UsuarioParticipacionDto>();

        public string ResumenEjecutivo { get; set; }
    }

    public class ReporteEjecutivoService
    {
        private readonly ReporteEjecutivoDao _dao;

        public ReporteEjecutivoService()
        {
            _dao = new ReporteEjecutivoDao();
        }

        public ReporteEjecutivoViewModel ObtenerReporte(string codInventario)
        {
            Response responseInfo = _dao.ObtenerInfo(codInventario);

            if (responseInfo.HUBO_ERROR) { throw new Exception(responseInfo.MENSAJE_ERROR); }

            InfoInventarioDao info = responseInfo.Entity as InfoInventarioDao;

            if (info == null)
            {
                throw new InvalidOperationException("No existe el inventario " + codInventario);
            }

            Response responseProductos = _dao.ObtenerProductosKpi(codInventario);

            if (responseProductos.HUBO_ERROR)
            { throw new Exception(responseProductos.MENSAJE_ERROR); }

            ProductosKpiDao productos = responseProductos.Entity as ProductosKpiDao;
            Response responseUbicaciones = _dao.ObtenerUbicacionesConDiferencia(codInventario);

            if (responseUbicaciones.HUBO_ERROR)
            {
                throw new Exception(responseUbicaciones.MENSAJE_ERROR);
            }

            UbicacionesResultDao ubicaciones = responseUbicaciones.Entity as UbicacionesResultDao;
            Response responseDiferencias = _dao.ObtenerDiferencias(codInventario);

            if (responseDiferencias.HUBO_ERROR) { throw new Exception(responseDiferencias.MENSAJE_ERROR); }

            DiferenciasResultDao diferencias = responseDiferencias.Entity as DiferenciasResultDao;
            Response responseUsuarios = _dao.ObtenerUsuarios(codInventario);

            if (responseUsuarios.HUBO_ERROR) { throw new Exception(responseUsuarios.MENSAJE_ERROR); }

            UsuariosResultDao usuarios = responseUsuarios.Entity as UsuariosResultDao;
            ReporteEjecutivoViewModel vm = new ReporteEjecutivoViewModel();

            vm.CodInventario = codInventario; 
            vm.CodAlmacen = info.CodAlmacen;
            vm.DscAlmacen = info.DscAlmacen;
            vm.FechaInicio = info.FechaInicio;
            vm.FechaCierreFinal = info.FechaCierreFinal;
            vm.DuracionDias = info.DuracionDias;
            vm.Estado = info.Estado;
            vm.ConteosRealizados = info.ConteosRealizados;
            vm.ProductosInventariados = productos.ProductosInventariados;
            vm.ProductosConDiferencia = productos.ProductosConDiferencia;
            vm.UbicacionesConDiferencias = ubicaciones.UbicacionesConDiferencias;
            vm.Top10Ubicaciones = ubicaciones.Top10;
            vm.SumaSobrantes = diferencias.SumaSobrantes;
            vm.SumaFaltantes = diferencias.SumaFaltantes;
            vm.DiferenciaNeta = diferencias.DiferenciaNeta;
            vm.Top10Productos = diferencias.Top10;
            vm.UsuariosParticipantes = usuarios.UsuariosParticipantes;
            vm.TopUsuarios = usuarios.Top10;
            vm.ResumenEjecutivo = ArmarResumenTexto(vm);
            return vm;
        }

        private string ArmarResumenTexto(ReporteEjecutivoViewModel vm)
        {
            var top1Producto = vm.Top10Productos.FirstOrDefault();
            var top3Ubicaciones = vm.Top10Ubicaciones.Take(3).Select(u => u.Codigo).ToList();

            var sb = new StringBuilder();
            sb.Append($"Se inventariaron {vm.ProductosInventariados:N0} productos, alcanzando una exactitud global del {vm.ExactitudPct:0.00}%. ");
            sb.Append($"De estos, {vm.ProductosConDiferencia:N0} presentaron diferencias. ");
            sb.Append($"Se registraron {vm.SumaSobrantes:N0} unidades sobrantes y {Math.Abs(vm.SumaFaltantes):N0} unidades faltantes, ");
            sb.Append($"resultando en una diferencia neta de {vm.DiferenciaNeta:N0} unidades. ");
            sb.Append($"El conteo fue realizado por {vm.UsuariosParticipantes} usuarios. ");

            if (top3Ubicaciones.Any())
                sb.Append($"Las ubicaciones con mayor concentración de diferencias fueron {string.Join(", ", top3Ubicaciones)}. ");

            if (top1Producto != null)
                sb.Append($"El producto con la mayor diferencia individual fue \"{top1Producto.DscProducto}\" con {top1Producto.Diferencia:N0} unidades.");

            return sb.ToString();
        }
    }
}
