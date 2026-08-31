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
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
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
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
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
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();
                        reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            reporteUsuarioBE.ProductosInventariados = (reader["PRODUCTOS_INVENTARIADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_INVENTARIADOS"].ToString());
                            reporteUsuarioBE.UsuariosParticipantes = (reader["USUARIOS_PARTICIPANTES"] == DBNull.Value) ? 0 : Int32.Parse(reader["USUARIOS_PARTICIPANTES"].ToString());
                            reporteUsuarioBE.ProductosFueraInventario = (reader["PRODUCTOS_FUERA_INVENTARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_FUERA_INVENTARIO"].ToString());
                            reporteUsuarioBE.UsuarioMasLecturas = (reader["USUARIO_MAS_LECTURAS"] == DBNull.Value) ? string.Empty : reader["USUARIO_MAS_LECTURAS"].ToString();
                            reporteUsuarioBE.UsuarioMenosLecturas = (reader["USUARIOS_MENOS_LECTURAS"] == DBNull.Value) ? string.Empty : reader["USUARIOS_MENOS_LECTURAS"].ToString();
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
                        reporteUsuarioBE.TblReporteUsuarios = listaTabla;
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


        public Response ReporteProducto(
            string codInventario,
            string busqueda = "",
            int estado = 0,
            int observacion = 0,
            string start = "0",
            string length = "10",
            string order = null)
        {
            Response response = new Response();
            // MIS BE
            ReporteProductoBE reporteProductoBE = new ReporteProductoBE();
            List<TblReporteProductoBE> listaTabla = new List<TblReporteProductoBE>();
            List<decimal> totalesFooter = new List<decimal>();


            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_reporteXproducto_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        // Parametros SP
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 20).Value = busqueda;
                        comando.Parameters.Add("@P_ESTADO", SqlDbType.Int).Value = estado;
                        comando.Parameters.Add("@P_OBSERVACION", SqlDbType.Int).Value = observacion;

                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

                        conexion.Open();
                        reader = comando.ExecuteReader();

                        if (reader.Read())
                        {
                            reporteProductoBE.ProductosInventariados = (reader["PRODUCTOS_INVENTARIADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_INVENTARIADOS"].ToString());
                            reporteProductoBE.ProductosConDiferencia = (reader["PRODUCTOS_CON_DIFERENCIA"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_CON_DIFERENCIA"].ToString());
                            reporteProductoBE.ProductosSinDiferencia = (reader["PRODUCTOS_SIN_DIFERENCIA"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_SIN_DIFERENCIA"].ToString());
                            reporteProductoBE.ProductosNoLecturados = (reader["PRODUCTOS_NO_LECTURADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_NO_LECTURADOS"].ToString());
                            reporteProductoBE.ProductosUbicacionDiferente = (reader["PRODUCTOS_UBICACION_DIFERENTE"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_UBICACION_DIFERENTE"].ToString());
                            reporteProductoBE.ProductosLoteDiferente = (reader["PRODUCTOS_LOTE_DIFERENTE"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_LOTE_DIFERENTE"].ToString());
                            reporteProductoBE.StockInicialTotal = (reader["STOCK_INICIAL_TOTAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL_TOTAL"].ToString());
                            reporteProductoBE.StockFinalTotal = (reader["STOCK_FINAL_TOTAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL_TOTAL"].ToString());
                            reporteProductoBE.DiferenciaTotal = (reader["DIFERENCIA_TOTAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIA_TOTAL"].ToString());
                            reporteProductoBE.EstadoConteo = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["ESTADO_INVENTARIO"].ToString();
                            reporteProductoBE.ConteoSeleccionado = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString());
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                TblReporteProductoBE filaTabla = new TblReporteProductoBE();

                                filaTabla.CodProducto = (reader["CODIGO"] == DBNull.Value) ? string.Empty : reader["CODIGO"].ToString();
                                filaTabla.DscProducto = (reader["PRODUCTO"] == DBNull.Value) ? string.Empty : reader["PRODUCTO"].ToString();
                                filaTabla.UbicacionInicial = (reader["UBICACION_INICIAL"] == DBNull.Value) ? string.Empty : reader["UBICACION_INICIAL"].ToString();
                                filaTabla.UbicacionContada = (reader["UBICACION_CONTADA"] == DBNull.Value) ? string.Empty : reader["UBICACION_CONTADA"].ToString();
                                filaTabla.LoteInicial = (reader["LOTE_INICIAL"] == DBNull.Value) ? string.Empty : reader["LOTE_INICIAL"].ToString();
                                filaTabla.LoteContado = (reader["LOTE_CONTADO"] == DBNull.Value) ? string.Empty : reader["LOTE_CONTADO"].ToString();
                                filaTabla.StockInicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
                                filaTabla.StockContado = (reader["STOCK_CONTADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTADO"].ToString());
                                filaTabla.Diferencia = (reader["DIFERENCIA"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIA"].ToString());
                                filaTabla.Observacion = (reader["OBSERVACION"] == DBNull.Value) ? string.Empty : reader["OBSERVACION"].ToString();
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
                        reporteProductoBE.TblReporteProductos = listaTabla;
                        reader.Close();
                    }
                }

                response.Entity = reporteProductoBE;
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


        public Response ReporteUbicacion(
            string codInventario,
            string busqueda = "",
            string p_estado = "",
            int diferencias = 0,
            int p_idstart = 0,
            int p_length = 10,
            string p_order = null)
        {
            Response response = new Response();
            ReporteUbicacionBE reporteUbicacionBE = new ReporteUbicacionBE();
            List<TblReporteUbicacionBE> listaTabla = new List<TblReporteUbicacionBE>();
            List<decimal> totalesFooter = new List<decimal>();


            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_reporteXubicaciones_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = busqueda;
                        comando.Parameters.Add("@P_ESTADO", SqlDbType.VarChar, 20).Value = (object)p_estado ?? DBNull.Value;
                        comando.Parameters.Add("@P_SOLO_DIFERENCIAS", SqlDbType.Int).Value = diferencias;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = p_idstart;
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = p_length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = (object)p_order ?? DBNull.Value;

                        conexion.Open();
                        reader = comando.ExecuteReader();



                        if (reader.Read())
                        {
                            reporteUbicacionBE.Ubicaciones_Totales = (reader["UBICACIONES_TOTALES"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_TOTALES"].ToString());
                            reporteUbicacionBE.Ubicaciones_Nuevas = (reader["UBICACIONES_NUEVAS"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_NUEVAS"].ToString());
                            reporteUbicacionBE.Ubicaciones_Completas = (reader["UBICACIONES_COMPLETAS"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_COMPLETAS"].ToString());
                            reporteUbicacionBE.Ubicaciones_En_Proceso = (reader["UBICACIONES_EN_PROCESO"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_EN_PROCESO"].ToString());
                            reporteUbicacionBE.Ubicaciones_No_Iniciadas = (reader["UBICACIONES_NO_INICIADAS"] == DBNull.Value) ? 0 : Int32.Parse(reader["UBICACIONES_NO_INICIADAS"].ToString());
                            reporteUbicacionBE.Estado_Inventario = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["ESTADO_INVENTARIO"].ToString();
                            reporteUbicacionBE.Cod_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["COD_INVENTARIO"].ToString();
                            reporteUbicacionBE.Conteo_Actual = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString());
                            reporteUbicacionBE.Ultimo_Conteo_Finalizado = (reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
                            reporteUbicacionBE.Top3_Mayor_Diferencia = (reader["TOP3_MAYOR_DIFERENCIA"] == DBNull.Value) ? string.Empty : reader["TOP3_MAYOR_DIFERENCIA"].ToString();
                            reporteUbicacionBE.Top3_Menor_Diferencia = (reader["TOP3_MENOR_DIFERENCIA"] == DBNull.Value) ? string.Empty : reader["TOP3_MENOR_DIFERENCIA"].ToString();
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                TblReporteUbicacionBE filaTabla = new TblReporteUbicacionBE();

                                filaTabla.Cod_Ubicacion = (reader["CODIGO"] == DBNull.Value) ? string.Empty : reader["CODIGO"].ToString();
                                filaTabla.Descripcion_Ubicacion = (reader["UBICACION"] == DBNull.Value) ? string.Empty : reader["UBICACION"].ToString();
                                filaTabla.Productos_Totales = (reader["PRODUCTOS_TOTALES"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PRODUCTOS_TOTALES"].ToString());
                                filaTabla.Stock_Inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString()); // stock inicial error
                                filaTabla.Productos_Contados = (reader["PRODUCTOS_CONTADOS"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PRODUCTOS_CONTADOS"].ToString());
                                filaTabla.Stock_Contado = (reader["STOCK_CONTADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTADO"].ToString());
                                filaTabla.Diferencia = (reader["DIFERENCIA"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIA"].ToString());
                                filaTabla.Estado = (reader["ESTADO"] == DBNull.Value) ? string.Empty : reader["ESTADO"].ToString();

                                listaTabla.Add(filaTabla);
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                totalesFooter.Add((reader["StockTotalProductos"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockTotalProductos"].ToString()));
                                totalesFooter.Add((reader["StockInicialTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockInicialTotal"].ToString()));
                                totalesFooter.Add((reader["StockFinalTotal"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockFinalTotal"].ToString()));
                                totalesFooter.Add((reader["StockDiferencial"] == DBNull.Value) ? 0 : decimal.Parse(reader["StockDiferencial"].ToString()));
                            }
                        }
                        reporteUbicacionBE.TblReporteUbicacion = listaTabla;
                        reader.Close();
                    }
                }

                response.Entity = reporteUbicacionBE;
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


        public Response ReporteAuditoria(
            string codInventario,
            string busqueda = "",
            string estado = "",
            int start = 0,
            int p_length = 10,
            string p_order = null)
        {
            Response response = new Response();
            ReporteAuditoriaBE reporteAuditoriaBE = new ReporteAuditoriaBE();
            List<TblReporteAuditoriaBE> listaTabla = new List<TblReporteAuditoriaBE>();
            List<decimal> totalesFooter = new List<decimal>();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("SP_WEB_ReporteAuditoria__2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = 120; // Establecer un tiempo de espera de 120 segundos
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = busqueda;
                        comando.Parameters.Add("@ESTADO", SqlDbType.VarChar, 20).Value = (object)estado ?? DBNull.Value;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = start;
                        //comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = p_length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = (object)p_order ?? DBNull.Value;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                reporteAuditoriaBE.Productos_Inventariados = (reader["PRODUCTOS_INVENTARIADOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_INVENTARIADOS"].ToString());
                                reporteAuditoriaBE.Productos_Con_Diferencia = (reader["PRODUCTOS_CON_DIFERENCIA"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_CON_DIFERENCIA"].ToString());
                                reporteAuditoriaBE.Productos_Sin_Diferencia = (reader["PRODUCTOS_SIN_DIFERENCIA"] == DBNull.Value) ? 0 : Int32.Parse(reader["PRODUCTOS_SIN_DIFERENCIA"].ToString());
                                reporteAuditoriaBE.Usuarios_Participantes = (reader["USUARIOS_PARTICIPANTES"] == DBNull.Value) ? 0 : Int32.Parse(reader["USUARIOS_PARTICIPANTES"].ToString());
                                reporteAuditoriaBE.Primera_Lectura = (reader["PRIMERA_LECTURA"] == DBNull.Value) ? string.Empty : reader["PRIMERA_LECTURA"].ToString();
                                reporteAuditoriaBE.Ultima_Lectura = (reader["ULTIMA_LECTURA"] == DBNull.Value) ? string.Empty : reader["ULTIMA_LECTURA"].ToString();
                                reporteAuditoriaBE.TiempoTotalInventario = (reader["DURACION_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["DURACION_INVENTARIO"].ToString();
                                reporteAuditoriaBE.Estado_Inventario = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["ESTADO_INVENTARIO"].ToString();
                                reporteAuditoriaBE.Codigo_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? string.Empty : reader["COD_INVENTARIO"].ToString();
                                reporteAuditoriaBE.Conteo_Actual = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString()); //Total de conteos Realizados
                            }

                            if (reader.NextResult())
                            {
                                var nroConteo = new List<int>(); // crear lista para guardar numeros de conteo / xd solo existen 3 xd
                                for (int i = 0; i < reader.FieldCount; i++)// obtenemos las columnas que tiene el resultado del SP y las guardamos en la lista de conteos 
                                {
                                    string colName = reader.GetName(i);
                                    //obtener el campo de Conteo1_Stock, Conteo2_Stock, Conteo3_Stock y asi sucesivamente
                                    if (colName.StartsWith("Conteo") && colName.EndsWith("_Stock"))
                                    {
                                        string numeroStr = colName.Replace("Conteo", "").Replace("_Stock", "");
                                        //aca ahora lo convertimos de string a int para guardarlo en la lista de conteos 
                                        if (int.TryParse(numeroStr, out int n))
                                        {
                                            nroConteo.Add(n);
                                        }
                                    }
                                }
                                nroConteo.Sort();//ordenar de menor a mayor p mi ñaño

                                while (reader.Read())
                                {
                                    var item = new TblReporteAuditoriaBE
                                    {

                                        Codigo = (reader["CODIGO"] == DBNull.Value) ? string.Empty : reader["CODIGO"].ToString(),
                                        Producto = (reader["PRODUCTO"] == DBNull.Value) ? string.Empty : reader["PRODUCTO"].ToString(),
                                        Stock_Inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString()),
                                        Ubicacion_Inicial = (reader["UBICACION_INICIAL"] == DBNull.Value) ? string.Empty : reader["UBICACION_INICIAL"].ToString(),
                                        Lote_Inicial = (reader["LOTE_INICIAL"] == DBNull.Value) ? string.Empty : reader["LOTE_INICIAL"].ToString(),
                                        Stock_Final = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL"].ToString()),
                                        Diferencia = (reader["DIFERENCIA"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["DIFERENCIA"].ToString()),
                                        Estado = (reader["ESTADO"] == DBNull.Value) ? string.Empty : reader["ESTADO"].ToString(),
                                        //aqui se ingresara la nueva tabla uwu 
                                        Conteos = new List<ConteoDetalleBE>()
                                    };

                                    foreach (int n in nroConteo)
                                    {
                                        string colStock = $"Conteo{n}_Stock";

                                        if (reader[colStock] == DBNull.Value) continue; // si este producto no tiene stock en este conteo no agregues el conteo u pasa al siguiente xd

                                        item.Conteos.Add(new ConteoDetalleBE
                                        {
                                            // NroConteo no existe en las tablas pero sirve colocarlo para tener referencia a todos los demas campos de la tabla a que
                                            // // numero de conteo pertenecen pue amore 
                                            Nro_Conteo = n,
                                            Stock_Contado = Convert.ToDecimal(reader[colStock]),
                                            Ubicaciones_Contadas = (reader[$"Conteo{n}_Ubicacion"] == DBNull.Value) ? string.Empty : reader[$"Conteo{n}_Ubicacion"].ToString(),
                                            Lote_Contado = (reader[$"Conteo{n}_Lote"] == DBNull.Value) ? string.Empty : reader[$"Conteo{n}_Lote"].ToString(),
                                            Usuario = (reader[$"Conteo{n}_Usuario"] == DBNull.Value) ? string.Empty : reader[$"Conteo{n}_Usuario"].ToString(),
                                            Hora_Registro = (reader[$"Conteo{n}_Hora"] == DBNull.Value) ? string.Empty : reader[$"Conteo{n}_Hora"].ToString()
                                        });
                                    }
                                    listaTabla.Add(item);
                                }
                            }
                            reporteAuditoriaBE.TblReporteAuditoria = listaTabla;

                            //nuevo footer crj por que acepte
                            FooterAuditoriaBE footer = new FooterAuditoriaBE
                            {
                                TotalesPorConteo = new List<ConteoTotalBE>()
                            };

                            if (reader.NextResult())
                            {
                                // detectar cantidad de columnas que es un copy y pega de como esta estructurado conteos p 
                                var nroConteoFooter = new List<int>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string colName = reader.GetName(i);
                                    if (colName.StartsWith("Conteo") && colName.EndsWith("_Total"))
                                    {
                                        string numeroStr = colName.Replace("Conteo", "").Replace("_Total", "");
                                        if (int.TryParse(numeroStr, out int n)) nroConteoFooter.Add(n);
                                    }
                                }
                                nroConteoFooter.Sort();

                                if (reader.Read())
                                {
                                    footer.StockInicialTotal = (reader["StockInicialTotal"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["StockInicialTotal"]);
                                    footer.StockFinalTotal = (reader["StockFinalTotal"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["StockFinalTotal"]);
                                    footer.StockDiferencial = (reader["StockDiferencial"] == DBNull.Value) ? 0 : Convert.ToDecimal(reader["StockDiferencial"]);

                                    foreach (int n in nroConteoFooter)
                                    {
                                        string colTotal = $"Conteo{n}_Total";
                                        footer.TotalesPorConteo.Add(new ConteoTotalBE
                                        {
                                            Nro_ConteoTotal = n,
                                            Total_Stock = (reader[colTotal] == DBNull.Value) ? (decimal?)null : Convert.ToDecimal(reader[colTotal])
                                        });
                                    }
                                }
                            }
                            reporteAuditoriaBE.TblReporteAuditoria = listaTabla;
                            reporteAuditoriaBE.Footer = footer;
                            // reader.Close(); al usar using no es necesario cerrar el reader ya que se cierra automaticamente al salir del bloque
                        }
                    }
                }
                response.Entity = reporteAuditoriaBE;
                //response.footerTable = totalesFooter;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.ToString();
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


        public Response ReporteValorizado(
            string codInventario,
            string busqueda = "",
            int p_impacto = 0,
            string moneda = "",
            string p_order = null
            )
        {
            Response response = new Response();
            ReporteValorizadoBE reporteValorizadoBE = new ReporteValorizadoBE();
            List<TblReporteValorizadoBE> listaTabla = new List<TblReporteValorizadoBE>();
            FooterValorizadoBE totalesFooter = new FooterValorizadoBE();
            List<TblMonedas> ListaMonedas = new List<TblMonedas>();

            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            try
            {
                using (cn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_ReporteValorizado_2026", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = codInventario;
                        cmd.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar).Value = busqueda;
                        cmd.Parameters.Add("@P_IMPACTO", SqlDbType.Int).Value = p_impacto;
                        cmd.Parameters.Add("@P_MONEDA", SqlDbType.VarChar).Value = moneda;
                        cmd.Parameters.Add("@P_ORDER", SqlDbType.VarChar).Value = p_order;

                        cn.Open();
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            TblMonedas filaMoneda = new TblMonedas();
                            filaMoneda.CodMoneda = (dr["COD_MONEDA"] == DBNull.Value) ? string.Empty : dr["COD_MONEDA"].ToString();
                            filaMoneda.DscMoneda = (dr["DSC_MONEDA"] == DBNull.Value) ? string.Empty : dr["DSC_MONEDA"].ToString();
                            filaMoneda.CantidadProducto = (dr["CANTIDAD_PRODUCTO"] == DBNull.Value) ? 0 : Int32.Parse(dr["CANTIDAD_PRODUCTO"].ToString());

                            ListaMonedas.Add(filaMoneda);
                        }

                        if (dr.NextResult())
                        {
                            if (dr.Read())
                            {
                                reporteValorizadoBE.Codigo_Inventario = (dr["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : dr["COD_INVENTARIO"].ToString();
                                reporteValorizadoBE.Valorizado_Total_Inventariado = (dr["VALOR_TOTAL_INVENTARIO"] == DBNull.Value) ? 0 : decimal.Parse(dr["VALOR_TOTAL_INVENTARIO"].ToString());
                                reporteValorizadoBE.Valor_final_Inventariado = (dr["VALOR_FINAL_INVENTARIO"] == DBNull.Value) ? 0 : decimal.Parse(dr["VALOR_FINAL_INVENTARIO"].ToString());
                                reporteValorizadoBE.Impacto_Economico_Neto = (dr["IMPACTO_ECONOMICO_NETO"] == DBNull.Value) ? 0 : decimal.Parse(dr["IMPACTO_ECONOMICO_NETO"].ToString());
                                reporteValorizadoBE.Valorizado_Sobrantes = (dr["VALOR_TOTAL_SOBRANTES"] == DBNull.Value) ? 0 : decimal.Parse(dr["VALOR_TOTAL_SOBRANTES"].ToString());
                                reporteValorizadoBE.Valorizado_Faltantes = (dr["VALOR_TOTAL_FALTANTES"] == DBNull.Value) ? 0 : decimal.Parse(dr["VALOR_TOTAL_FALTANTES"].ToString());
                                reporteValorizadoBE.Cantidad_Productos_Impacto = (dr["CANT_PRODUCTOS_IMPACTO"] == DBNull.Value) ? 0 : Int32.Parse(dr["CANT_PRODUCTOS_IMPACTO"].ToString());
                                reporteValorizadoBE.Cantidad_Productos_Sin_Costo = (dr["CANT_PRODUCTOS_SIN_COSTO"] == DBNull.Value) ? 0 : Int32.Parse(dr["CANT_PRODUCTOS_SIN_COSTO"].ToString());
                                reporteValorizadoBE.Estado_Inventario = (dr["ESTADO_INVENTARIO"] == DBNull.Value) ? String.Empty : dr["ESTADO_INVENTARIO"].ToString();
                                reporteValorizadoBE.ConteoActual = (dr["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(dr["CONTEO_ACTUAL"].ToString());
                            }
                        }
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                TblReporteValorizadoBE filaTabla = new TblReporteValorizadoBE();
                                filaTabla.Codigo = (dr["CODIGO"] == DBNull.Value) ? String.Empty : dr["CODIGO"].ToString();
                                filaTabla.Producto = (dr["PRODUCTO"] == DBNull.Value) ? String.Empty : dr["PRODUCTO"].ToString();
                                filaTabla.Ubicacion_Inicial = (dr["UBICACION_INICIAL"] == DBNull.Value) ? String.Empty : dr["UBICACION_INICIAL"].ToString();
                                filaTabla.Lote_Inicial = (dr["LOTE_INICIAL"] == DBNull.Value) ? String.Empty : dr["LOTE_INICIAL"].ToString();
                                filaTabla.Cod_Moneda = (dr["COD_MONEDA"] == DBNull.Value) ? String.Empty : dr["COD_MONEDA"].ToString();
                                filaTabla.Costo_Unitario = (dr["COSTO_UNITARIO"] == DBNull.Value) ? 0 : decimal.Parse(dr["COSTO_UNITARIO"].ToString());
                                filaTabla.Stock_Inicial = (dr["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(dr["STOCK_INICIAL"].ToString());
                                filaTabla.Stock_Final = (dr["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(dr["STOCK_FINAL"].ToString());
                                filaTabla.Diferencia = (dr["DIFERENCIA"] == DBNull.Value) ? 0 : decimal.Parse(dr["DIFERENCIA"].ToString());
                                filaTabla.Impacto_Economico = (dr["IMPACTO_ECONOMICO"] == DBNull.Value) ? 0 : decimal.Parse(dr["IMPACTO_ECONOMICO"].ToString());
                                filaTabla.Observacion = (dr["OBSERVACION"] == DBNull.Value) ? String.Empty : dr["OBSERVACION"].ToString();

                                listaTabla.Add(filaTabla);
                            }
                        }
                        if (dr.NextResult())
                        {
                            if (dr.Read())
                            {
                                totalesFooter.StockInicialTotal = (dr["StockInicialTotal"] == DBNull.Value) ? 0 : decimal.Parse(dr["StockInicialTotal"].ToString());
                                totalesFooter.StockFinalTotal = (dr["StockFinalTotal"] == DBNull.Value) ? 0 : decimal.Parse(dr["StockFinalTotal"].ToString());
                                totalesFooter.StockDiferencial = (dr["StockDiferencial"] == DBNull.Value) ? 0 : decimal.Parse(dr["StockDiferencial"].ToString());
                                totalesFooter.ImpactoEconomicoTotal = (dr["ImpactoEconomicoTotal"] == DBNull.Value) ? 0 : decimal.Parse(dr["ImpactoEconomicoTotal"].ToString());
                            }
                        }
                        reporteValorizadoBE.tblReporteValorizadoBE = listaTabla;
                        reporteValorizadoBE.tblMonedas = ListaMonedas;
                        reporteValorizadoBE.Footer = totalesFooter;
                        dr.Close();
                    }
                }
                response.Entity = reporteValorizadoBE;
                response.footerTable = totalesFooter;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally { dr?.Close(); }
            return response;
        }










        // EN DESUSO

        //public Response ObtenerReporteInventarioCerrado(
        //string codInventario,
        //int filtroDiferencias, // 0=Todos, 1=Con diferencias, 2=Sin diferencias
        //string start,
        //string length,
        //string order,
        //string search)
        //{
        //    Response response = new Response();
        //    List<decimal> Lista_Footer = new List<decimal>();
        //    List<TblReportePrincipalBE> Lista_result = new List<TblReportePrincipalBE>();
        //    List<string> Lista_InfoInventario = new List<string>();
        //    SqlConnection conexion = null;
        //    SqlCommand comando = null;
        //    SqlDataReader reader = null;

        //    try
        //    {
        //        using (conexion = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_CERRADO", conexion))
        //            {
        //                comando.CommandType = CommandType.StoredProcedure;
        //                comando.Parameters.Clear();

        //                comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
        //                comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
        //                comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
        //                comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;
        //                comando.Parameters.Add("@P_Search", SqlDbType.VarChar, 500).Value = search ?? "";
        //                comando.Parameters.Add("@FILTRO_DIFERENCIAS", SqlDbType.Int).Value = filtroDiferencias;

        //                conexion.Open();

        //                using (reader = comando.ExecuteReader())
        //                {
        //                    // Primer ResultSet: RESUMEN (totales)
        //                    while (reader.Read())
        //                    {
        //                        response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

        //                        // Totales para el footer
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_CONTEO_1"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_1"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_CONTEO_2"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_2"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_CONTEO_3"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_CONTEO_3"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_FINAL"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

        //                        // Estadísticas adicionales
        //                        Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
        //                        Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));
        //                        //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_POSITIVO"].ToString()));
        //                        //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_NEGATIVO"].ToString()));
        //                        //Lista_Footer.Add((reader["TOTAL_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_POSITIVO"].ToString()));
        //                        //Lista_Footer.Add((reader["TOTAL_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_NEGATIVO"].ToString()));

        //                        // Nueva lista para info del inventario
        //                        Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_CIERRE_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["ESTADO_DESCRIPCION"] == DBNull.Value) ? String.Empty : reader["ESTADO_DESCRIPCION"].ToString());
        //                    }

        //                    // Segundo ResultSet: DETALLE (registros)
        //                    reader.NextResult();
        //                    while (reader.Read())
        //                    {
        //                        TblReportePrincipalBE entity = new TblReportePrincipalBE();

        //                        entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
        //                        entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
        //                        entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        //                        entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
        //                        entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

        //                        // Stocks y conteos
        //                        entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
        //                        entity.Conteo_1 = (reader["CONTEO_1"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_1"].ToString());
        //                        entity.Conteo_2 = (reader["CONTEO_2"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_2"].ToString());
        //                        entity.Conteo_3 = (reader["CONTEO_3"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_3"].ToString());
        //                        entity.Ultimo_Conteo = (reader["ULTIMO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO"].ToString());
        //                        entity.Stock_Final = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL"].ToString());
        //                        entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());

        //                        // Estado del ajuste
        //                        entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

        //                        // Usuario y fechas
        //                        entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
        //                        entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString();
        //                        entity.Fch_Cierre = (reader["FCH_CIERRE"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE"].ToString();

        //                        // Análisis y observaciones
        //                        entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
        //                        entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

        //                        Lista_result.Add(entity);
        //                    }
        //                }
        //            }
        //        }

        //        response.Entity = Lista_result;
        //        response.footerTable = Lista_Footer;
        //        response.infoInventario = Lista_InfoInventario; // Nueva propiedad

        //    }
        //    catch (Exception ex)
        //    {
        //        response.MENSAJE_ERROR = ex.Message.ToString();
        //        response.HUBO_ERROR = true;
        //    }
        //    finally
        //    {
        //        if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        //        if (comando != null) comando.Dispose();
        //        if (reader != null) reader.Dispose();
        //    }

        //    return response;
        //}


        ///// <summary>
        ///// Obtiene reporte de inventario por tipo (DIFERENCIAL, PRODUCTO, UBICACION, USUARIO)
        ///// Siempre usa el último conteo FINALIZADO (no el actual en proceso)
        ///// </summary>
        ///// <param name="codInventario">Código del inventario</param>
        ///// <param name="tipoReporte">DIFERENCIAL, PRODUCTO, UBICACION, USUARIO</param>
        ///// <param name="busqueda">Texto de búsqueda (aplica según tipo de reporte)</param>
        ///// <param name="start">Inicio de paginación</param>
        ///// <param name="length">Cantidad de registros por página</param>
        ///// <param name="order">Orden de columnas</param>
        ///// <returns>Response con datos del reporte</returns>
        //public Response ObtenerReporteInventarioV2(
        //    string codInventario,
        //    string tipoReporte = "DIFERENCIAL", // DIFERENCIAL, PRODUCTO, UBICACION, USUARIO
        //    string busqueda = "",
        //    string start = "0",
        //    string length = "10",
        //    string order = null)
        //{
        //    Response response = new Response();
        //    List<decimal> Lista_Footer = new List<decimal>();
        //    List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
        //    List<string> Lista_InfoInventario = new List<string>();
        //    SqlConnection conexion = null;
        //    SqlCommand comando = null;
        //    SqlDataReader reader = null;

        //    try
        //    {
        //        using (conexion = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_V2", conexion))
        //            {
        //                comando.CommandType = CommandType.StoredProcedure;
        //                comando.Parameters.Clear();

        //                // Parámetros del SP
        //                comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
        //                comando.Parameters.Add("@TIPO_REPORTE", SqlDbType.VarChar, 20).Value = tipoReporte.ToUpper();
        //                comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = busqueda ?? "";
        //                comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
        //                comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);

        //                // Si no se especifica orden, el SP usa su orden por defecto según tipo
        //                if (!string.IsNullOrEmpty(order))
        //                {
        //                    comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;
        //                }

        //                conexion.Open();

        //                using (reader = comando.ExecuteReader())
        //                {
        //                    // ===================================================
        //                    // Primer ResultSet: RESUMEN (totales y estadísticas)
        //                    // ===================================================
        //                    while (reader.Read())
        //                    {
        //                        response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

        //                        // Totales para el footer
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_ULTIMO_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_ULTIMO_CONTEO"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

        //                        // Estadísticas de productos
        //                        Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
        //                        Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));
        //                        //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_POSITIVO"].ToString()));
        //                        //Lista_Footer.Add((reader["PRODUCTOS_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_AJUSTE_NEGATIVO"].ToString()));

        //                        // Totales de ajustes
        //                        //Lista_Footer.Add((reader["TOTAL_AJUSTE_POSITIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_POSITIVO"].ToString()));
        //                        //Lista_Footer.Add((reader["TOTAL_AJUSTE_NEGATIVO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_AJUSTE_NEGATIVO"].ToString()));

        //                        // Información del inventario
        //                        Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_CIERRE_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["ESTADO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["ESTADO_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
        //                        Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
        //                        Lista_InfoInventario.Add((reader["TIPO_REPORTE"] == DBNull.Value) ? String.Empty : reader["TIPO_REPORTE"].ToString());
        //                    }

        //                    // ===================================================
        //                    // Segundo ResultSet: DETALLE (registros paginados)
        //                    // ===================================================
        //                    reader.NextResult();
        //                    while (reader.Read())
        //                    {
        //                        TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

        //                        // Datos del producto
        //                        entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
        //                        entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
        //                        entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        //                        entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
        //                        entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

        //                        // Stocks y conteos (ahora solo usa el último conteo finalizado)
        //                        entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
        //                        entity.Ultimo_Conteo_Finalizado = (reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
        //                        entity.Stock_Ultimo_Conteo = (reader["STOCK_ULTIMO_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_ULTIMO_CONTEO"].ToString());
        //                        entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());

        //                        // Estado del ajuste
        //                        entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

        //                        // Usuario y fechas
        //                        entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
        //                        entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString();
        //                        entity.Fch_Cierre = (reader["FCH_CIERRE"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE"].ToString();

        //                        // Análisis y observaciones
        //                        entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
        //                        entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

        //                        // Estado del inventario y conteo actual
        //                        entity.Estado_Inventario = (reader["ESTADO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["ESTADO_INVENTARIO"].ToString();
        //                        entity.Conteo_Actual = (reader["CONTEO_ACTUAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_ACTUAL"].ToString());

        //                        Lista_result.Add(entity);
        //                    }
        //                }
        //            }
        //        }

        //        response.Entity = Lista_result;
        //        response.footerTable = Lista_Footer;
        //        response.infoInventario = Lista_InfoInventario;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.MENSAJE_ERROR = ex.Message.ToString();
        //        response.HUBO_ERROR = true;
        //    }
        //    finally
        //    {
        //        if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        //        if (comando != null) comando.Dispose();
        //        if (reader != null) reader.Dispose();
        //    }

        //    return response;
        //}

        //// ===================================================
        //// MÉTODOS DE ACCESO DIRECTO POR TIPO DE REPORTE
        //// ===================================================

        ///// <summary>
        ///// Obtiene solo productos con diferencias
        ///// </summary>
        //public Response ObtenerReporteDiferencial(string codInventario, string start = "0", string length = "50", string order = null)
        //{
        //    return ObtenerReporteInventarioV2(codInventario, "DIFERENCIAL", "", start, length, order);
        //}

        ///// <summary>
        ///// Obtiene reporte filtrado por producto
        ///// </summary>
        //public Response ObtenerReporteProducto(string codInventario, string busquedaProducto, string start = "0", string length = "50", string order = null)
        //{
        //    return ObtenerReporteInventarioV2(codInventario, "PRODUCTO", busquedaProducto, start, length, order);
        //}

        ///// <summary>
        ///// Obtiene reporte filtrado por ubicación
        ///// </summary>
        //public Response ObtenerReporteUbicacion(string codInventario, string busquedaUbicacion, string start = "0", string length = "50", string order = null)
        //{
        //    return ObtenerReporteInventarioV2(codInventario, "UBICACION", busquedaUbicacion, start, length, order);
        //}

        ///// <summary>
        ///// Obtiene reporte filtrado por usuario
        ///// </summary>
        //public Response ObtenerReporteUsuario(string codInventario, string busquedaUsuario, string start = "0", string length = "50", string order = null)
        //{
        //    return ObtenerReporteInventarioV2(codInventario, "USUARIO", busquedaUsuario, start, length, order);
        //}


        ////public Response ObtenerReporteInventarioPorConteo(
        ////string codInventario,
        ////int nroConteo,
        ////string start = "0",
        ////string length = "10",
        ////string order = "COD_PRODUCTO ASC")
        ////    {
        ////        Response response = new Response();
        ////        List<decimal> Lista_Footer = new List<decimal>();
        ////        List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
        ////        List<string> Lista_InfoInventario = new List<string>();
        ////        SqlConnection conexion = null;
        ////        SqlCommand comando = null;
        ////        SqlDataReader reader = null;

        ////        try
        ////        {
        ////            using (conexion = new SqlConnection(Connection.AppStringConection()))
        ////            {
        ////                using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_POR_CONTEO", conexion))
        ////                {
        ////                    comando.CommandType = CommandType.StoredProcedure;
        ////                    comando.Parameters.Clear();

        ////                    comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
        ////                    comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
        ////                    comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = "";
        ////                    comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
        ////                    comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
        ////                    comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

        ////                    conexion.Open();

        ////                    using (reader = comando.ExecuteReader())
        ////                    {
        ////                        // Primer ResultSet: RESUMEN
        ////                        if (reader.HasRows)  // ⭐ Validar si tiene filas
        ////                        {
        ////                            while (reader.Read())
        ////                            {
        ////                                response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

        ////                                Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
        ////                                Lista_Footer.Add((reader["TOTAL_STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_CONTEO"].ToString()));
        ////                                Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));
        ////                                Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
        ////                                Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));

        ////                                Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
        ////                                Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
        ////                                Lista_InfoInventario.Add((reader["FCH_CIERRE_CONTEO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_CONTEO"].ToString());
        ////                                Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
        ////                                Lista_InfoInventario.Add((reader["NRO_CONTEO"] == DBNull.Value) ? "0" : reader["NRO_CONTEO"].ToString());
        ////                                Lista_InfoInventario.Add((reader["ESTADO_CONTEO"] == DBNull.Value) ? String.Empty : reader["ESTADO_CONTEO"].ToString());
        ////                                Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
        ////                                Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());
        ////                            }
        ////                        }
        ////                        else
        ////                        {
        ////                            // ⭐ Si no hay filas en el primer ResultSet, establecer count = 0
        ////                            response.count = 0;
        ////                        }

        ////                        // Segundo ResultSet: DETALLE
        ////                        if (reader.NextResult())  // ⭐ Validar si hay segundo ResultSet
        ////                        {
        ////                            while (reader.Read())
        ////                            {
        ////                                TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

        ////                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
        ////                                entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
        ////                                entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        ////                                entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
        ////                                entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();
        ////                                entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
        ////                                entity.Ultimo_Conteo_Finalizado = (reader["NRO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["NRO_CONTEO"].ToString());
        ////                                entity.Stock_Ultimo_Conteo = (reader["STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTEO"].ToString());
        ////                                entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString());
        ////                                entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();
        ////                                entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
        ////                                entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_LECTURA"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_LECTURA"].ToString();
        ////                                entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
        ////                                entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

        ////                                Lista_result.Add(entity);
        ////                            }
        ////                        }
        ////                    }
        ////                }
        ////            }

        ////            response.Entity = Lista_result;
        ////            response.footerTable = Lista_Footer;
        ////            response.infoInventario = Lista_InfoInventario;
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            response.MENSAJE_ERROR = ex.Message.ToString();
        ////            response.HUBO_ERROR = true;
        ////        }
        ////        finally
        ////        {
        ////            if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        ////            if (comando != null) comando.Dispose();
        ////            if (reader != null) reader.Dispose();
        ////        }

        ////        return response;
        ////    }
        //public Response ObtenerReporteInventarioPorConteo(
        //string codInventario,
        //int nroConteo,
        //string start = "0",
        //string length = "10",
        //string order = "COD_PRODUCTO ASC")
        //{
        //    Response response = new Response();
        //    List<decimal> Lista_Footer = new List<decimal>();
        //    List<TblReportePrincipalV2BE> Lista_result = new List<TblReportePrincipalV2BE>();
        //    List<string> Lista_InfoInventario = new List<string>();
        //    SqlConnection conexion = null;
        //    SqlCommand comando = null;
        //    SqlDataReader reader = null;

        //    try
        //    {
        //        using (conexion = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (comando = new SqlCommand("SP_REPORTE_INVENTARIO_POR_CONTEO", conexion))
        //            {
        //                comando.CommandType = CommandType.StoredProcedure;
        //                comando.Parameters.Clear();

        //                // Parámetros del SP
        //                comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
        //                comando.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = nroConteo;
        //                comando.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = ""; // Por ahora vacío
        //                comando.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = int.Parse(start);
        //                comando.Parameters.Add("@P_LENGTH", SqlDbType.Int).Value = int.Parse(length);
        //                comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = order;

        //                conexion.Open();

        //                using (reader = comando.ExecuteReader())
        //                {
        //                    // ===================================================
        //                    // Primer ResultSet: RESUMEN (totales y estadísticas)
        //                    // ===================================================
        //                    while (reader.Read())
        //                    {
        //                        response.count = (reader["FILAS_TOTAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["FILAS_TOTAL"].ToString());

        //                        // Totales para el footer
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_INICIAL"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_STOCK_CONTEO"].ToString()));
        //                        Lista_Footer.Add((reader["TOTAL_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["TOTAL_DIFERENCIAL"].ToString()));

        //                        // Estadísticas de productos
        //                        Lista_Footer.Add((reader["PRODUCTOS_CON_DIFERENCIAS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_CON_DIFERENCIAS"].ToString()));
        //                        Lista_Footer.Add((reader["PRODUCTOS_EXACTOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["PRODUCTOS_EXACTOS"].ToString()));

        //                        // Información del inventario
        //                        Lista_InfoInventario.Add((reader["NOMBRE_ALMACEN"] == DBNull.Value) ? String.Empty : reader["NOMBRE_ALMACEN"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_REGISTRO_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["FCH_CIERRE_CONTEO"] == DBNull.Value) ? String.Empty : reader["FCH_CIERRE_CONTEO"].ToString());
        //                        Lista_InfoInventario.Add((reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString());
        //                        Lista_InfoInventario.Add((reader["NRO_CONTEO"] == DBNull.Value) ? "0" : reader["NRO_CONTEO"].ToString());
        //                        Lista_InfoInventario.Add((reader["ESTADO_CONTEO"] == DBNull.Value) ? String.Empty : reader["ESTADO_CONTEO"].ToString());
        //                        Lista_InfoInventario.Add((reader["CONTEO_ACTUAL"] == DBNull.Value) ? "0" : reader["CONTEO_ACTUAL"].ToString());
        //                        Lista_InfoInventario.Add((reader["ULTIMO_CONTEO_FINALIZADO"] == DBNull.Value) ? "0" : reader["ULTIMO_CONTEO_FINALIZADO"].ToString());

        //                    }

        //                    // ===================================================
        //                    // Segundo ResultSet: DETALLE (registros paginados)
        //                    // ===================================================
        //                    reader.NextResult();
        //                    while (reader.Read())
        //                    {

        //                        TblReportePrincipalV2BE entity = new TblReportePrincipalV2BE();

        //                        // Datos del producto
        //                        entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
        //                        entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
        //                        entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        //                        entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
        //                        entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();

        //                        // Stocks y conteos - AJUSTAR PARA COMPATIBILIDAD
        //                        entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString());
        //                        entity.Ultimo_Conteo_Finalizado = (reader["NRO_CONTEO"] == DBNull.Value) ? 0 : Int32.Parse(reader["NRO_CONTEO"].ToString()); // Mapear aquí
        //                        entity.Stock_Ultimo_Conteo = (reader["STOCK_CONTEO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_CONTEO"].ToString()); // Mapear aquí
        //                        entity.Stock_Diferencial = (reader["DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["DIFERENCIAL"].ToString()); // Mapear aquí

        //                        // Estado del ajuste
        //                        entity.Estado_Ajuste = (reader["ESTADO_AJUSTE"] == DBNull.Value) ? String.Empty : reader["ESTADO_AJUSTE"].ToString();

        //                        // Usuario y fechas
        //                        entity.Cod_Usuario_Registro = (reader["USUARIO_CONTEO"] == DBNull.Value) ? String.Empty : reader["USUARIO_CONTEO"].ToString();
        //                        entity.Fch_Registro_Inventario = (reader["FCH_REGISTRO_LECTURA"] == DBNull.Value) ? String.Empty : reader["FCH_REGISTRO_LECTURA"].ToString();

        //                        // Análisis y observaciones
        //                        entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
        //                        entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();

        //                        Lista_result.Add(entity);
        //                    }
        //                }
        //            }
        //        }

        //        response.Entity = Lista_result;
        //        response.footerTable = Lista_Footer;
        //        response.infoInventario = Lista_InfoInventario;

        //    }
        //    catch (Exception ex)
        //    {
        //        response.MENSAJE_ERROR = ex.Message.ToString();
        //        response.HUBO_ERROR = true;
        //    }
        //    finally
        //    {
        //        if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        //        if (comando != null) comando.Dispose();
        //        if (reader != null) reader.Dispose();
        //    }

        //    return response;
        //}
    }
}