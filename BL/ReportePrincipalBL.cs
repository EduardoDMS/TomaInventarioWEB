using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ReportePrincipalBL
    {

        public Response ObtenerReporteInventarioCerrado(
        string codInventario,
        int filtroDiferencias, // 0=Todos, 1=Con diferencias, 2=Sin diferencias
        string start,
        string length,
        string order,
        string search)
        {
            Response response = new Response();
            try
            {
                response = new ReportePrincipalDAO().ObtenerReporteInventarioCerrado(
                    codInventario,
                    filtroDiferencias,
                    start,
                    length,
                    order,
                    search);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }

      
        /// <summary>
        /// Obtiene reporte de inventario V2 - Usa último conteo finalizado
        /// </summary>
        /// <param name="codInventario">Código del inventario</param>
        /// <param name="tipoReporte">DIFERENCIAL, PRODUCTO, UBICACION, USUARIO</param>
        /// <param name="busqueda">Texto de búsqueda según tipo de reporte</param>
        /// <param name="start">Inicio de paginación</param>
        /// <param name="length">Cantidad de registros</param>
        /// <param name="order">Orden de columnas</param>
        /// <returns>Response con datos del reporte</returns>
        public Response ObtenerReporteInventarioV2(
            string codInventario,
            string tipoReporte = "DIFERENCIAL",
            string busqueda = "",
            string start = "0",
            string length = "10",
            string order = null)
        {
            Response response = new Response();
            try
            {
                // Validar tipo de reporte
                string[] tiposValidos = { "DIFERENCIAL", "PRODUCTO", "UBICACION", "USUARIO" };
                if (!tiposValidos.Contains(tipoReporte.ToUpper()))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "Tipo de reporte inválido. Use: DIFERENCIAL, PRODUCTO, UBICACION o USUARIO";
                    return response;
                }

                // Validar código de inventario
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteInventarioV2(
                    codInventario,
                    tipoReporte.ToUpper(),
                    busqueda ?? "",
                    start,
                    length,
                    order);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Obtiene reporte solo de productos con DIFERENCIAS
        /// </summary>
        public Response ObtenerReporteDiferencial(
            string codInventario,
            string start = "0",
            string length = "50",
            string order = null)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteDiferencial(
                    codInventario,
                    start,
                    length,
                    order);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Obtiene reporte filtrado por PRODUCTO
        /// </summary>
        public Response ObtenerReporteProducto(
            string codInventario,
            string busquedaProducto,
            string start = "0",
            string length = "50",
            string order = null)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteProducto(
                    codInventario,
                    busquedaProducto ?? "",
                    start,
                    length,
                    order);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Obtiene reporte filtrado por UBICACIÓN
        /// </summary>
        public Response ObtenerReporteUbicacion(
            string codInventario,
            string busquedaUbicacion,
            string start = "0",
            string length = "50",
            string order = null)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteUbicacion(
                    codInventario,
                    busquedaUbicacion ?? "",
                    start,
                    length,
                    order);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Obtiene reporte filtrado por USUARIO
        /// </summary>
        public Response ObtenerReporteUsuario(
            string codInventario,
            string busquedaUsuario,
            string start = "0",
            string length = "50",
            string order = null)
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteUsuario(
                    codInventario,
                    busquedaUsuario ?? "",
                    start,
                    length,
                    order);
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }




        //public Response ObtenerReporteInventarioPorConteo(
        //string codInventario,
        //int nroConteo,
        //string start = "0",
        //string length = "10",
        //string order = "COD_PRODUCTO ASC")
        //    {
        //        Response response = new Response();
        //        try
        //        {
        //            // Validar código de inventario
        //            if (string.IsNullOrWhiteSpace(codInventario))
        //            {
        //                response.HUBO_ERROR = true;
        //                response.MENSAJE_ERROR = "El código de inventario es obligatorio";
        //                return response;
        //            }

        //            // Validar número de conteo
        //            if (nroConteo <= 0)
        //            {
        //                response.HUBO_ERROR = true;
        //                response.MENSAJE_ERROR = "El número de conteo debe ser mayor a 0";
        //                return response;
        //            }

        //            // Llamar al DAO
        //            response = new ReportePrincipalDAO().ObtenerReporteInventarioPorConteo(
        //                codInventario,
        //                nroConteo,
        //                start,
        //                length,
        //                order);

        //        }
        //        catch (Exception ex)
        //        {
        //            response.HUBO_ERROR = true;
        //            response.MENSAJE_ERROR = ex.Message;
        //        }
        //        return response;
        //    }
        public Response ObtenerReporteInventarioPorConteo(
            string codInventario,
            int nroConteo,
            string start = "0",
            string length = "10",
            string order = "COD_PRODUCTO ASC")
        {
            Response response = new Response();
            try
            {
                if (string.IsNullOrWhiteSpace(codInventario))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El código de inventario es obligatorio";
                    return response;
                }

                if (nroConteo <= 0)
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El número de conteo debe ser mayor a 0";
                    return response;
                }

                response = new ReportePrincipalDAO().ObtenerReporteInventarioPorConteo(
                    codInventario,
                    nroConteo,
                    start,
                    length,
                    order);

                // ⭐ CLAVE: Si count = 0, marcar como error ANTES de devolver
                if (response.count == 0)
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = $"El conteo #{nroConteo} no existe o no está cerrado";
                    // ⭐ NO cambiar count aquí, dejarlo en 0
                }
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }
    }
    }
      

