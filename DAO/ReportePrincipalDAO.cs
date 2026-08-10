using BE;
using BE.Reportes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class ReportePrincipalDAO
    {
        // ==================================================
        // NUEVOS MÉTODOS DE REPORTES
        // ==================================================

        public Response ReporteDiferencial(
            string codInventario,
            string estado,
            string tipoDiferencia,
            string busqueda = "",
            string start = "0",
            string length = "10",
            string order = null)
        {
            Response response = new Response();
            // MIS BE
            ReporteDiferencialBE reporteDiferencialBE = new ReporteDiferencialBE();
            List<TblReporteDiferencialBE> listaTabla = new List<TblReporteDiferencialBE>();
            List<decimal> totalesFooter = new List<decimal>();


            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_ReporteDiferencial_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parametros SP
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 20).Value = busqueda;
                        comando.Parameters.Add("@ESTADO", SqlDbType.VarChar, 20).Value = estado.ToUpper();
                        comando.Parameters.Add("@TIPO_DIFERENCIA", SqlDbType.VarChar, 20).Value = tipoDiferencia.ToUpper();
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();
                        reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            reporteDiferencialBE.ProductosInventariados = (reader["ProductosInventariados"] == DBNull.Value) ? 0 : Int32.Parse(reader["ProductosInventariados"].ToString());
                            reporteDiferencialBE.ProductosConDiferencia = (reader["ProductosConDiferencia"] == DBNull.Value) ? 0 : Int32.Parse(reader["ProductosConDiferencia"].ToString());
                            reporteDiferencialBE.ProductosSinDiferencia = (reader["ProductosSinDiferencia"] == DBNull.Value) ? 0 : Int32.Parse(reader["ProductosSinDiferencia"].ToString());
                            reporteDiferencialBE.TotalDeSobrantes = (reader["ProductosSobrantes"] == DBNull.Value) ? 0 : decimal.Parse(reader["ProductosSobrantes"].ToString());
                            reporteDiferencialBE.TotalDeFaltantes = (reader["ProductosFaltantes"] == DBNull.Value) ? 0 : decimal.Parse(reader["ProductosFaltantes"].ToString());
                            reporteDiferencialBE.DiferenciaNetaInventario = (reader["DiferenciaNetaInventario"] == DBNull.Value) ? 0 : decimal.Parse(reader["DiferenciaNetaInventario"].ToString());
                            reporteDiferencialBE.EstadoInventario = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["ESTADO_INVENTARIO"].ToString();
                            reporteDiferencialBE.ConteoActual = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString());
                            reporteDiferencialBE.UltimoConteoFinalizado = (reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                TblReporteDiferencialBE filaTabla = new TblReporteDiferencialBE();

                                filaTabla.CodProducto = (reader["COD_PRODUCTO"] == DBNull.Value) ? string.Empty : reader["COD_PRODUCTO"].ToString();
                                //filaTabla.LoteProducto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? string.Empty : reader["LOTE_PRODUCTO"].ToString();
                                filaTabla.DscProducto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? string.Empty : reader["DSC_PRODUCTO"].ToString();
                                //filaTabla.CodUbicacion = (reader["COD_UBICACION"] == DBNull.Value) ? string.Empty : reader["COD_UBICACION"].ToString();
                                filaTabla.StockInicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                filaTabla.StockFinal = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL"].ToString());
                                filaTabla.StockDiferencial = (reader["DIFERENCIA"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIA"].ToString());
                                filaTabla.Estado = (reader["ESTADO"] == DBNull.Value) ? string.Empty : reader["ESTADO"].ToString();
                                filaTabla.TipoDiferencia = (reader["TIPO_DIFERENCIA"] == DBNull.Value) ? string.Empty : reader["TIPO_DIFERENCIA"].ToString();

                                listaTabla.Add(filaTabla);
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                totalesFooter.Add((reader["StockInicialTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockInicialTotal"].ToString()));
                                totalesFooter.Add((reader["StockFinalTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockFinalTotal"].ToString()));
                                totalesFooter.Add((reader["StockDiferencial"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockDiferencial"].ToString()));
                            }
                        }
                        reporteDiferencialBE.TblReporteDiferencial = listaTabla;
                        reader.Close();
                    }
                }

                response.Entity = reporteDiferencialBE;
                response.footerTable = totalesFooter;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (reader != null) reader.Dispose();
                if (comando != null) comando.Dispose();
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
            }

            return response;
        }


        public Response ReporteConteo(
            string codInventario,
            int nroConteo = 1,
            string busqueda = "",
            string start = "0",
            string length = "10",
            string order = null)
        {
            Response response = new Response();
            // MIS BE
            ReporteConteoBE reporteConteoBE = new ReporteConteoBE();
            List<TblReporteConteoBE> listaTabla = new List<TblReporteConteoBE>();
            List<decimal> totalesFooter = new List<decimal>();


            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_reporteXconteo_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parametros SP
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 20).Value = busqueda;

                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();
                        reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            reporteConteoBE.ProductosInventariados = (reader["PRODUCTOS_INVENTARIADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_INVENTARIADOS"].ToString());
                            reporteConteoBE.ConteoSeleccionado = (reader["CONTEO_SELECCIONADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_SELECCIONADO"].ToString());
                            reporteConteoBE.LecturasRealizadas = (reader["LECTURAS_REALIZADAS"] == DBNull.Value) ? 0 : Int32.Parse(reader["LECTURAS_REALIZADAS"].ToString());
                            reporteConteoBE.UsuariosParticipantes = (reader["USUARIOS_PARTICIPANTES"] == DBNull.Value) ? 0 : Int32.Parse(reader["USUARIOS_PARTICIPANTES"].ToString());
                            reporteConteoBE.ProductosFueraInventario = (reader["PRODUCTOS_FUERA_INVENTARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_FUERA_INVENTARIO"].ToString());
                            reporteConteoBE.HoraPrimeraLectura = (reader["HORA_PRIMERA_LECTURA"] == DBNull.Value) ? string.Empty : reader["HORA_PRIMERA_LECTURA"].ToString();
                            reporteConteoBE.HoraUltimaLectura = (reader["HORA_ULTIMA_LECTURA"] == DBNull.Value) ? string.Empty : reader["HORA_ULTIMA_LECTURA"].ToString();
                            reporteConteoBE.DuracionConteo = (reader["DURACION_CONTEO"] == DBNull.Value) ? string.Empty : reader["DURACION_CONTEO"].ToString();
                            reporteConteoBE.EstadoConteo = (reader["COD_ESTADO_CONTEO"] == DBNull.Value) ? string.Empty : reader["COD_ESTADO_CONTEO"].ToString();
                            reporteConteoBE.ConteosDisponibles = (reader["CONTEOS_DISPONIBLES"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEOS_DISPONIBLES"].ToString());
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                TblReporteConteoBE filaTabla = new TblReporteConteoBE();

                                filaTabla.CodProducto = (reader["COD_PRODUCTO"] == DBNull.Value) ? string.Empty : reader["COD_PRODUCTO"].ToString();
                                filaTabla.DscProducto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? string.Empty : reader["DSC_PRODUCTO"].ToString();
                                filaTabla.UbicacionInicial = (reader["UBICACION_INICIAL"] == DBNull.Value) ? string.Empty : reader["UBICACION_INICIAL"].ToString();
                                filaTabla.UbicacionContada = (reader["UBICACION_CONTADA"] == DBNull.Value) ? string.Empty : reader["UBICACION_CONTADA"].ToString();
                                filaTabla.LoteInicial = (reader["LOTE_INICIAL"] == DBNull.Value) ? string.Empty : reader["LOTE_INICIAL"].ToString();
                                filaTabla.LoteContado = (reader["LOTE_CONTADO"] == DBNull.Value) ? string.Empty : reader["LOTE_CONTADO"].ToString();
                                filaTabla.StockInicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                filaTabla.StockContado = (reader["STOCK_CONTADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTADO"].ToString());
                                filaTabla.StockDiferencial = (reader["DIFERENCIA"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIA"].ToString());
                                filaTabla.Usuario = (reader["USUARIOS"] == DBNull.Value) ? string.Empty : reader["USUARIOS"].ToString();

                                listaTabla.Add(filaTabla);
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                totalesFooter.Add((reader["StockInicialTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockInicialTotal"].ToString()));
                                totalesFooter.Add((reader["StockFinalTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockFinalTotal"].ToString()));
                                totalesFooter.Add((reader["StockDiferencial"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockDiferencial"].ToString()));
                            }
                        }
                        reporteConteoBE.TblReporteConteo = listaTabla;
                        reader.Close();
                    }
                }

                response.Entity = reporteConteoBE;
                response.footerTable = totalesFooter;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (reader != null) reader.Dispose();
                if (comando != null) comando.Dispose();
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
            }

            return response;
        }


        public Response ReporteUsuario(
            string codInventario,
            int nroConteo = 1,
            string busqueda = "",
            string start = "0",
            string length = "10",
            string order = null)
        {
            Response response = new Response();
            // MIS BE
            ReporteUsuarioBE reporteUsuarioBE = new ReporteUsuarioBE();
            List<TblReporteUsuarioBE> listaTabla = new List<TblReporteUsuarioBE>();
            List<decimal> totalesFooter = new List<decimal>();


            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_reporteXusuarios_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parametros SP
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 20).Value = busqueda;

                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();
                        reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            reporteUsuarioBE.ProductosInventariados = (reader["PRODUCTOS_INVENTARIADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_INVENTARIADOS"].ToString());
                            reporteUsuarioBE.UsuariosParticipantes = (reader["USUARIOS_PARTICIPANTES"] == DBNull.Value) ? 0 : Int32.Parse(reader["USUARIOS_PARTICIPANTES"].ToString());
                            reporteUsuarioBE.ProductosFueraInventario = (reader["PRODUCTOS_FUERA_INVENTARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_FUERA_INVENTARIO"].ToString());
                            reporteUsuarioBE.UsuarioMasLecturas = (reader["USUARIO_MAS_LECTURAS"] == DBNull.Value) ? string.Empty : reader["USUARIO_MAS_LECTURAS"].ToString();
                            reporteUsuarioBE.UsuarioMasLecturas = (reader["USUARIOS_MENOS_LECTURAS"] == DBNull.Value) ? string.Empty : reader["USUARIO_MENOS_LECTURAS"].ToString();
                            reporteUsuarioBE.ConteoSeleccionado = (reader["CONTEO_SELECCIONADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_SELECCIONADO"].ToString());
                            reporteUsuarioBE.EstadoConteo = (reader["COD_ESTADO_CONTEO"] == DBNull.Value) ? string.Empty : reader["COD_ESTADO_CONTEO"].ToString();
                            reporteUsuarioBE.ConteosDisponibles = (reader["CONTEOS_DISPONIBLES"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEOS_DISPONIBLES"].ToString());
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                TblReporteUsuarioBE filaTabla = new TblReporteUsuarioBE();

                                filaTabla.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? string.Empty : reader["COD_USUARIO"].ToString();
                                filaTabla.NombreCompleto = (reader["NOMBRE_COMPLETO"] == DBNull.Value) ? string.Empty : reader["NOMBRE_COMPLETO"].ToString();
                                filaTabla.ProductosLecturados = (reader["PRODUCTOS_LECTURADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_LECTURADOS"].ToString());
                                filaTabla.UbicacionesLecturadas = (reader["UBICACIONES_LECTURADAS"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_LECTURADAS"].ToString());
                                filaTabla.StockTotalContado = (reader["STOCK_TOTAL_LECTURADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_TOTAL_LECTURADO"].ToString());
                                filaTabla.FechaHoraInicial = (reader["FECHA_HORA_INICIAL"] == DBNull.Value) ? string.Empty : reader["FECHA_HORA_INICIAL"].ToString();
                                filaTabla.FechaHoraFinal = (reader["FECHA_HORA_FINAL"] == DBNull.Value) ? string.Empty : reader["FECHA_HORA_FINAL"].ToString();
                                filaTabla.PromedioLecturas = (reader["PROMEDIO_LECTURAS"] == DBNull.Value) ? string.Empty : reader["PROMEDIO_LECTURAS"].ToString();
                                filaTabla.TiempoParticipacion = (reader["TIEMPO_PARTICIPACION"] == DBNull.Value) ? string.Empty : reader["TIEMPO_PARTICIPACION"].ToString();

                                listaTabla.Add(filaTabla);
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                totalesFooter.Add((reader["TOTALIZADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTALIZADO"].ToString()));
                            }
                        }
                        reporteUsuarioBE.TblreporteUsuarios = listaTabla;
                        reader.Close();
                    }
                }

                response.Entity = reporteUsuarioBE;
                response.footerTable = totalesFooter;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (reader != null) reader.Dispose();
                if (comando != null) comando.Dispose();
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
            }

            return response;
        }


























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




