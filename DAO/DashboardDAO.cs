using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class DashboardDAO
    {
        /// <summary>
        /// Obtiene todos los datos necesarios para el dashboard
        /// </summary>
        /// <param name="idEmpresa">ID de la empresa (opcional, null para todas)</param>
        /// <param name="anio">Año a consultar (opcional, null para año actual)</param>
        /// <returns>Response con DashboardCompletoBE</returns>
        public Response GetDatosDashboard(int? idEmpresa = null, int? anio = null)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            DashboardCompletoBE dashboard = new DashboardCompletoBE();

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_OBTENER_DATOS_DASHBOARD", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parámetros opcionales
                        if (idEmpresa.HasValue)
                            comando.Parameters.Add("@ID_EMPRESA", SqlDbType.Int).Value = idEmpresa.Value;
                        else
                            comando.Parameters.Add("@ID_EMPRESA", SqlDbType.Int).Value = DBNull.Value;

                        if (anio.HasValue)
                            comando.Parameters.Add("@ANIO", SqlDbType.Int).Value = anio.Value;
                        else
                            comando.Parameters.Add("@ANIO", SqlDbType.Int).Value = DBNull.Value;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            // RESULTADO 1: KPIs Principales
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dashboard.KPIs.TotalAlmacenes = reader["TotalAlmacenes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalAlmacenes"]);
                                    dashboard.KPIs.TotalInventarios = reader["TotalInventarios"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalInventarios"]);
                                    dashboard.KPIs.TotalProductos = reader["TotalProductos"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalProductos"]);
                                }
                            }

                            // RESULTADO 2: Inventarios por Mes
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    DashboardInventariosMesBE mes = new DashboardInventariosMesBE
                                    {
                                        NumMes = reader["NumMes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NumMes"]),
                                        NombreMes = reader["NombreMes"] == DBNull.Value ? String.Empty : reader["NombreMes"].ToString(),
                                        CantidadInventarios = reader["CantidadInventarios"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CantidadInventarios"])
                                    };
                                    dashboard.InventariosPorMes.Add(mes);
                                }
                            }

                            // RESULTADO 3: Últimos 6 Inventarios
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    DashboardUltimosInventariosBE inventario = new DashboardUltimosInventariosBE
                                    {
                                        ID_INVENTARIO = reader["ID_INVENTARIO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID_INVENTARIO"]),
                                        COD_INVENTARIO = reader["COD_INVENTARIO"] == DBNull.Value ? String.Empty : reader["COD_INVENTARIO"].ToString(),
                                        NombreAlmacen = reader["NombreAlmacen"] == DBNull.Value ? String.Empty : reader["NombreAlmacen"].ToString(),
                                        NRO_CONTEO = reader["NRO_CONTEO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NRO_CONTEO"]),
                                        EtiquetaConteo = reader["EtiquetaConteo"] == DBNull.Value ? String.Empty : reader["EtiquetaConteo"].ToString(),
                                        FechaFormato = reader["FechaFormato"] == DBNull.Value ? String.Empty : reader["FechaFormato"].ToString(),
                                        FCH_INICIO = reader["FCH_INICIO"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["FCH_INICIO"]),
                                        FCH_FIN = reader["FCH_FIN"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["FCH_FIN"]),
                                        COD_ESTADO = reader["COD_ESTADO"] == DBNull.Value ? String.Empty : reader["COD_ESTADO"].ToString(),
                                        DescripcionEstado = reader["DescripcionEstado"] == DBNull.Value ? String.Empty : reader["DescripcionEstado"].ToString(),
                                        ColorConteo = reader["ColorConteo"] == DBNull.Value ? String.Empty : reader["ColorConteo"].ToString(),
                                        ColorEstado = reader["ColorEstado"] == DBNull.Value ? String.Empty : reader["ColorEstado"].ToString()
                                    };
                                    dashboard.UltimosInventarios.Add(inventario);
                                }
                            }

                            // RESULTADO 4: Estadísticas Adicionales
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    dashboard.Estadisticas.TotalInventariosAnio = reader["TotalInventariosAnio"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalInventariosAnio"]);
                                    dashboard.Estadisticas.PromedioMensual = reader["PromedioMensual"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PromedioMensual"]);
                                    dashboard.Estadisticas.InventariosAbiertos = reader["InventariosAbiertos"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InventariosAbiertos"]);
                                    dashboard.Estadisticas.InventariosCerrados = reader["InventariosCerrados"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InventariosCerrados"]);
                                }
                            }
                        }
                    }
                }

                response.Entity = dashboard;
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (reader != null) reader.Dispose();
            }

            return response;
        }
    }
}