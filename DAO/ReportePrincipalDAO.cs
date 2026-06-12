using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using BE;

namespace DAO
{
    public class ReportePrincipalDAO
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
            List<decimal> Lista_Footer = new List<decimal>();
            List<TblReportePrincipalBE> Lista_result = new List<TblReportePrincipalBE>();
            List<string> Lista_InfoInventario = new List<string>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_CERRADO", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;
                        comando.Parameters.Add("@P_Search", SqlDbType.VarChar, 500).Value = search ?? "";
                        comando.Parameters.Add("@FILTRO_DIFERENCIAS", SqlDbType.Int).Value = filtroDiferencias;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            // Primer ResultSet: RESUMEN (totales)
                            while (reader.Read())
                            {
                                response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

                                // Totales para el footer
                                Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_CONTEO_1"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_1"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_CONTEO_2"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_2"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_CONTEO_3"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_3"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_FINAL"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

                                // Estadísticas adicionales
                                Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
                                Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));
                                //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_POSITIVO"].ToString()));
                                //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_NEGATIVO"].ToString()));
                                //Lista_Footer.Add((reader["TOTAL_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_POSITIVO"].ToString()));
                                //Lista_Footer.Add((reader["TOTAL_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_NEGATIVO"].ToString()));

                                // Nueva lista para info del inventario
                                Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
                                Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
                                Lista_InfoInventario.Add((reader["FCH_CIERRE_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_INVENTARIO"].ToString());
                                Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
                                Lista_InfoInventario.Add((reader["ESTADO_DESCRIPCION"] == DBNull.Value) ? String.Empty : reader["ESTADO_DESCRIPCION"].ToString());
                            }

                            // Segundo ResultSet: DETALLE (registros)
                            reader.NextResult();
                            while (reader.Read())
                            {
                                TblReportePrincipalBE entity = new TblReportePrincipalBE();

                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

                                // Stocks y conteos
                                entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                entity.Conteo_1 = (reader["CONTEO_1"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_1"].ToString());
                                entity.Conteo_2 = (reader["CONTEO_2"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_2"].ToString());
                                entity.Conteo_3 = (reader["CONTEO_3"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_3"].ToString());
                                entity.Ultimo_Conteo = (reader["ULTIMO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO"].ToString());
                                entity.Stock_Final = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL"].ToString());
                                entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());

                                // Estado del ajuste
                                entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

                                // Usuario y fechas
                                entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
                                entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString();
                                entity.Fch_Cierre = (reader["FCH_CIERRE"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE"].ToString();

                                // Análisis y observaciones
                                entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
                                entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

                                Lista_result.Add(entity);
                            }
                        }
                    }
                }

                response.Entity = Lista_result;
                response.footerTable = Lista_Footer;
                response.infoInventario = Lista_InfoInventario; // Nueva propiedad

            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return response;
        }




        
            /// <summary>
            /// Obtiene reporte de inventario por tipo (DIFERENCIAL, PRODUCTO, UBICACION, USUARIO)
            /// Siempre usa el último conteo FINALIZADO (no el actual en proceso)
            /// </summary>
            /// <param name="codInventario">Código del inventario</param>
            /// <param name="tipoReporte">DIFERENCIAL, PRODUCTO, UBICACION, USUARIO</param>
            /// <param name="busqueda">Texto de búsqueda (aplica según tipo de reporte)</param>
            /// <param name="start">Inicio de paginación</param>
            /// <param name="length">Cantidad de registros por página</param>
            /// <param name="order">Orden de columnas</param>
            /// <returns>Response con datos del reporte</returns>
            public Response ObtenerReporteInventarioV2(
                string codInventario,
                string tipoReporte = "DIFERENCIAL", // DIFERENCIAL, PRODUCTO, UBICACION, USUARIO
                string busqueda = "",
                string start = "0",
                string length = "10",
                string order = null)
            {
                Response response = new Response();
                List<decimal> Lista_Footer = new List<decimal>();
                List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
                List<string> Lista_InfoInventario = new List<string>();
                SqlConnection conexion = null;
                SqlCommand comando = null;
                SqlDataReader reader = null;

                try
                {
                    using (conexion = new SqlConnection(Connection.AppStringConection()))
                    {
                        using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_V2", conexion))
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.Parameters.Clear();

                            // Parámetros del SP
                            comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                            comando.Parameters.Add("@TIPO_REPORTE", SqlDbType.VarChar, 20).Value = tipoReporte.ToUpper();
                            comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = busqueda ?? "";
                            comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                            comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);

                            // Si no se especifica orden, el SP usa su orden por defecto según tipo
                            if (!string.IsNullOrEmpty(order))
                            {
                                comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;
                            }

                            conexion.Open();

                            using (reader = comando.ExecuteReader())
                            {
                                // ===================================================
                                // Primer ResultSet: RESUMEN (totales y estadísticas)
                                // ===================================================
                                while (reader.Read())
                                {
                                    response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

                                    // Totales para el footer
                                    Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
                                    Lista_Footer.Add((reader["TOTAL_STOCK_ULTIMO_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_ULTIMO_CONTEO"].ToString()));
                                    Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

                                    // Estadísticas de productos
                                    Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
                                    Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));
                                    //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_POSITIVO"].ToString()));
                                    //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_NEGATIVO"].ToString()));

                                    // Totales de ajustes
                                    //Lista_Footer.Add((reader["TOTAL_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_POSITIVO"].ToString()));
                                    //Lista_Footer.Add((reader["TOTAL_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_NEGATIVO"].ToString()));

                                    // Información del inventario
                                    Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
                                    Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
                                    Lista_InfoInventario.Add((reader["FCH_CIERRE_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_INVENTARIO"].ToString());
                                    Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
                                    Lista_InfoInventario.Add((reader["ESTADO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["ESTADO_INVENTARIO"].ToString());
                                    Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
                                    Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
                                    Lista_InfoInventario.Add((reader["TIPO_REPORTE"] == DBNull.Value) ? String.Empty : reader["TIPO_REPORTE"].ToString());
                                }

                                // ===================================================
                                // Segundo ResultSet: DETALLE (registros paginados)
                                // ===================================================
                                reader.NextResult();
                                while (reader.Read())
                                {
                                    TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

                                    // Datos del producto
                                    entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                    entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                    entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                    entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                    entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

                                    // Stocks y conteos (ahora solo usa el último conteo finalizado)
                                    entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                    entity.Ultimo_Conteo_Finalizado = (reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
                                    entity.Stock_Ultimo_Conteo = (reader["STOCK_ULTIMO_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_ULTIMO_CONTEO"].ToString());
                                    entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());

                                    // Estado del ajuste
                                    entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

                                    // Usuario y fechas
                                    entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
                                    entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString();
                                    entity.Fch_Cierre = (reader["FCH_CIERRE"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE"].ToString();

                                    // Análisis y observaciones
                                    entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
                                    entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

                                    // Estado del inventario y conteo actual
                                    entity.Estado_Inventario = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["ESTADO_INVENTARIO"].ToString();
                                    entity.Conteo_Actual = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString());

                                    Lista_result.Add(entity);
                                }
                            }
                        }
                    }

                    response.Entity = Lista_result;
                    response.footerTable = Lista_Footer;
                    response.infoInventario = Lista_InfoInventario;

                }
                catch (Exception ex)
                {
                    response.MENSAJE_ERROR = ex.Message.ToString();
                    response.HUBO_ERROR = true;
                }
                finally
                {
                    if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                    if (comando != null) comando.Dispose();
                    if (reader != null) reader.Dispose();
                }

                return response;
            }

            // ===================================================
            // MÉTODOS DE ACCESO DIRECTO POR TIPO DE REPORTE
            // ===================================================

            /// <summary>
            /// Obtiene solo productos con diferencias
            /// </summary>
            public Response ObtenerReporteDiferencial(string codInventario, string start = "0", string length = "50", string order = null)
            {
                return ObtenerReporteInventarioV2(codInventario, "DIFERENCIAL", "", start, length, order);
            }

            /// <summary>
            /// Obtiene reporte filtrado por producto
            /// </summary>
            public Response ObtenerReporteProducto(string codInventario, string busquedaProducto, string start = "0", string length = "50", string order = null)
            {
                return ObtenerReporteInventarioV2(codInventario, "PRODUCTO", busquedaProducto, start, length, order);
            }

            /// <summary>
            /// Obtiene reporte filtrado por ubicación
            /// </summary>
            public Response ObtenerReporteUbicacion(string codInventario, string busquedaUbicacion, string start = "0", string length = "50", string order = null)
            {
                return ObtenerReporteInventarioV2(codInventario, "UBICACION", busquedaUbicacion, start, length, order);
            }

            /// <summary>
            /// Obtiene reporte filtrado por usuario
            /// </summary>
            public Response ObtenerReporteUsuario(string codInventario, string busquedaUsuario, string start = "0", string length = "50", string order = null)
            {
                return ObtenerReporteInventarioV2(codInventario, "USUARIO", busquedaUsuario, start, length, order);
            }


        //public Response ObtenerReporteInventarioPorConteo(
        //string codInventario,
        //int nroConteo,
        //string start = "0",
        //string length = "10",
        //string order = "COD_PRODUCTO ASC")
        //    {
        //        Response response = new Response();
        //        List<decimal> Lista_Footer = new List<decimal>();
        //        List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
        //        List<string> Lista_InfoInventario = new List<string>();
        //        SqlConnection conexion = null;
        //        SqlCommand comando = null;
        //        SqlDataReader reader = null;

        //        try
        //        {
        //            using (conexion = new SqlConnection(Connection.AppStringConection()))
        //            {
        //                using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_POR_CONTEO", conexion))
        //                {
        //                    comando.CommandType = CommandType.StoredProcedure;
        //                    comando.Parameters.Clear();

        //                    comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
        //                    comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
        //                    comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = "";
        //                    comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
        //                    comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
        //                    comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

        //                    conexion.Open();

        //                    using (reader = comando.ExecuteReader())
        //                    {
        //                        // Primer ResultSet: RESUMEN
        //                        if (reader.HasRows)  // ⭐ Validar si tiene filas
        //                        {
        //                            while (reader.Read())
        //                            {
        //                                response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

        //                                Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
        //                                Lista_Footer.Add((reader["TOTAL_STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_CONTEO"].ToString()));
        //                                Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));
        //                                Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
        //                                Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));

        //                                Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
        //                                Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
        //                                Lista_InfoInventario.Add((reader["FCH_CIERRE_CONTEO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_CONTEO"].ToString());
        //                                Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
        //                                Lista_InfoInventario.Add((reader["NRO_CONTEO"] == DBNull.Value) ? "0" : reader["NRO_CONTEO"].ToString());
        //                                Lista_InfoInventario.Add((reader["ESTADO_CONTEO"] == DBNull.Value) ? String.Empty : reader["ESTADO_CONTEO"].ToString());
        //                                Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
        //                                Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
        //                            }
        //                        }
        //                        else
        //                        {
        //                            // ⭐ Si no hay filas en el primer ResultSet, establecer count = 0
        //                            response.count = 0;
        //                        }

        //                        // Segundo ResultSet: DETALLE
        //                        if (reader.NextResult())  // ⭐ Validar si hay segundo ResultSet
        //                        {
        //                            while (reader.Read())
        //                            {
        //                                TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

        //                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
        //                                entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
        //                                entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        //                                entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
        //                                entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();
        //                                entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
        //                                entity.Ultimo_Conteo_Finalizado = (reader["NRO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["NRO_CONTEO"].ToString());
        //                                entity.Stock_Ultimo_Conteo = (reader["STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTEO"].ToString());
        //                                entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());
        //                                entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();
        //                                entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
        //                                entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_LECTURA"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_LECTURA"].ToString();
        //                                entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
        //                                entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

        //                                Lista_result.Add(entity);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            response.Entity = Lista_result;
        //            response.footerTable = Lista_Footer;
        //            response.infoInventario = Lista_InfoInventario;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.MENSAJE_ERROR = ex.Message.ToString();
        //            response.HUBO_ERROR = true;
        //        }
        //        finally
        //        {
        //            if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        //            if (comando != null) comando.Dispose();
        //            if (reader != null) reader.Dispose();
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
            List<decimal> Lista_Footer = new List<decimal>();
            List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
            List<string> Lista_InfoInventario = new List<string>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_POR_CONTEO", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parámetros del SP
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = ""; // Por ahora vacío
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            // ===================================================
                            // Primer ResultSet: RESUMEN (totales y estadísticas)
                            // ===================================================
                            while (reader.Read())
                            {
                                response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

                                // Totales para el footer
                                Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_CONTEO"].ToString()));
                                Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

                                // Estadísticas de productos
                                Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
                                Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));

                                // Información del inventario
                                Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
                                Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
                                Lista_InfoInventario.Add((reader["FCH_CIERRE_CONTEO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_CONTEO"].ToString());
                                Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
                                Lista_InfoInventario.Add((reader["NRO_CONTEO"] == DBNull.Value) ? "0" : reader["NRO_CONTEO"].ToString());
                                Lista_InfoInventario.Add((reader["ESTADO_CONTEO"] == DBNull.Value) ? String.Empty : reader["ESTADO_CONTEO"].ToString());
                                Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
                                Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());

                            }

                            // ===================================================
                            // Segundo ResultSet: DETALLE (registros paginados)
                            // ===================================================
                            reader.NextResult();
                            while (reader.Read())
                            {

                                TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

                                // Datos del producto
                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

                                // Stocks y conteos - AJUSTAR PARA COMPATIBILIDAD
                                entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                entity.Ultimo_Conteo_Finalizado = (reader["NRO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["NRO_CONTEO"].ToString()); // Mapear aquí
                                entity.Stock_Ultimo_Conteo = (reader["STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTEO"].ToString()); // Mapear aquí
                                entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString()); // Mapear aquí

                                // Estado del ajuste
                                entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

                                // Usuario y fechas
                                entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
                                entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_LECTURA"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_LECTURA"].ToString();

                                // Análisis y observaciones
                                entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
                                entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

                                Lista_result.Add(entity);
                            }
                        }
                    }
                }

                response.Entity = Lista_result;
                response.footerTable = Lista_Footer;
                response.infoInventario = Lista_InfoInventario;

            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return response;
        }


    }
    }




