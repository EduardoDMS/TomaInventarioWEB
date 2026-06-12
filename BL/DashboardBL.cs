using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class DashboardBL
    {
        private DashboardDAO dashboardDAO;

        public DashboardBL()
        {
            dashboardDAO = new DashboardDAO();
        }

        /// <summary>
        /// Obtiene todos los datos del dashboard
        /// </summary>
        /// <param name="idEmpresa">ID de la empresa (opcional)</param>
        /// <param name="anio">Año a consultar (opcional)</param>
        /// <returns>Response con DashboardCompletoBE</returns>
        public Response ObtenerDatosDashboard(int? idEmpresa = null, int? anio = null)
        {
            Response response = new Response();

            try
            {
                // Validaciones de negocio (opcional)
                if (anio.HasValue && (anio.Value < 2000 || anio.Value > DateTime.Now.Year + 1))
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "El año debe estar entre 2000 y " + (DateTime.Now.Year + 1);
                    return response;
                }

                // Llamada al DAO
                response = dashboardDAO.GetDatosDashboard(idEmpresa, anio);

                // Validación adicional: si no hay datos
                if (!response.HUBO_ERROR && response.Entity != null)
                {
                    DashboardCompletoBE dashboard = (DashboardCompletoBE)response.Entity;

                    // Asegurar que siempre haya 12 meses en el gráfico
                    if (dashboard.InventariosPorMes.Count == 0)
                    {
                        // Crear meses vacíos si no hay datos
                        string[] meses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
                        for (int i = 1; i <= 12; i++)
                        {
                            dashboard.InventariosPorMes.Add(new DashboardInventariosMesBE
                            {
                                NumMes = i,
                                NombreMes = meses[i - 1],
                                CantidadInventarios = 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "Error en la capa de negocio: " + ex.Message;
            }

            return response;
        }

        /// <summary>
        ///  KPIs del dashboard
        /// </summary>
        public Response ObtenerKPIs(int? idEmpresa = null)
        {
            Response response = ObtenerDatosDashboard(idEmpresa);

            if (!response.HUBO_ERROR && response.Entity != null)
            {
                DashboardCompletoBE dashboard = (DashboardCompletoBE)response.Entity;
                response.Entity = dashboard.KPIs;
            }

            return response;
        }

        /// <summary>
        /// Últimos inventarios
        /// </summary>
        public Response ObtenerUltimosInventarios(int? idEmpresa = null)
        {
            Response response = ObtenerDatosDashboard(idEmpresa);

            if (!response.HUBO_ERROR && response.Entity != null)
            {
                DashboardCompletoBE dashboard = (DashboardCompletoBE)response.Entity;
                response.Entity = dashboard.UltimosInventarios;
            }

            return response;
        }
    }
}