using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace DAO
{
    public class InventarioDAO
    {
        public Response ListarInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3, string start, string length, string order, string search)
        {
            List<TblInventarioBE> Lista_result = new List<TblInventarioBE>();
            List<decimal> Lista_Footer = new List<decimal>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_GET_DETALLE_3", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = COD_INVENTARIO;
                        comando.Parameters.Add("@NRO_CONTEO_UNO", SqlDbType.Int).Value = NRO_CONTEO_1;
                        comando.Parameters.Add("@NRO_CONTEO_DOS", SqlDbType.Int).Value = NRO_CONTEO_2;
                        comando.Parameters.Add("@NRO_CONTEO_TRES", SqlDbType.Int).Value = NRO_CONTEO_3;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.VarChar, 20).Value = start;
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.VarChar, 20).Value = length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 20).Value = order;
                        comando.Parameters.Add("@P_Search", SqlDbType.VarChar, 500).Value = search;


                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());

                                Lista_Footer.Add((reader["Cantidad_Inicial"] == DBNull.Value) ? 0 : decimal.Parse(reader["Cantidad_Inicial"].ToString()));

                                Lista_Footer.Add((reader["Conteo1"] == DBNull.Value) ? 0 : decimal.Parse(reader["Conteo1"].ToString()));
                                Lista_Footer.Add((reader["Conteo2"] == DBNull.Value) ? 0 : decimal.Parse(reader["Conteo2"].ToString()));
                                Lista_Footer.Add((reader["Conteo3"] == DBNull.Value) ? 0 : decimal.Parse(reader["Conteo3"].ToString()));
                                Lista_Footer.Add((reader["Total"] == DBNull.Value) ? 0 : decimal.Parse(reader["Total"].ToString()));
                                Lista_Footer.Add((reader["Diferencial"] == DBNull.Value) ? 0 : decimal.Parse(reader["Diferencial"].ToString()));
                                Lista_Footer.Add((reader["Inventariado"] == DBNull.Value) ? 0 : Int32.Parse(reader["Inventariado"].ToString()));
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                TblInventarioBE entity = new TblInventarioBE();
                                entity.Cod_Empresa = (reader["COD_EMPRESA"] == DBNull.Value) ? String.Empty : reader["COD_EMPRESA"].ToString();
                                entity.Dsc_Empresa = (reader["DSC_EMPRESA"] == DBNull.Value) ? String.Empty : reader["DSC_EMPRESA"].ToString();
                                entity.Cod_Almacen = (reader["COD_ALMACEN"] == DBNull.Value) ? String.Empty : reader["COD_ALMACEN"].ToString();
                                entity.Dsc_Almacen = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
                                entity.Cod_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
                                entity.Id_producto = (reader["ID_PRODUCTO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_PRODUCTO"].ToString());
                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.Dsc_Producto = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.Lote_Producto = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                entity.Serie_Producto = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();


                                entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INICIAL"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                entity.Conteo_1 = (reader["CONTEO_UNO"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_UNO"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                entity.Conteo_2 = (reader["CONTEO_DOS"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_DOS"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                entity.Conteo_3 = (reader["CONTEO_TRE"] == DBNull.Value) ? 0 : decimal.Parse(reader["CONTEO_TRE"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                entity.Stock_Final = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_FINAL"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                                entity.Stock_Diferencial = (reader["STOCK_DIFERENCIAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_DIFERENCIAL"].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                                //entity.Stock_inicial = (reader["STOCK_INICIAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["STOCK_INICIAL"].ToString());
                                //entity.Conteo_1 = (reader["CONTEO_UNO"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_UNO"].ToString());
                                //entity.Conteo_2 = (reader["CONTEO_DOS"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_DOS"].ToString());
                                //entity.Conteo_3 = (reader["CONTEO_TRE"] == DBNull.Value) ? 0 : Int32.Parse(reader["CONTEO_TRE"].ToString());
                                //entity.Stock_Final = (reader["STOCK_FINAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["STOCK_FINAL"].ToString());
                                //entity.Stock_Diferencial = (reader["STOCK_DIFERENCIAL"] == DBNull.Value) ? 0 : Int32.Parse(reader["STOCK_DIFERENCIAL"].ToString());
                                entity.Id_ubicacion = (reader["ID_UBICACION"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_UBICACION"].ToString());
                                entity.Fch_inicio = (reader["FCH_INICIO"] == DBNull.Value) ? String.Empty : reader["FCH_INICIO"].ToString();
                                entity.Fch_fin = (reader["FCH_FIN"] == DBNull.Value) ? String.Empty : reader["FCH_FIN"].ToString();
                                entity.Cod_ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.Dsc_ubicacion = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();

                                entity.FLG_ESNUEVO = (reader["FLG_ESNUEVO"] == DBNull.Value) ? String.Empty : reader["FLG_ESNUEVO"].ToString();

                                //IQFARMA
                                //entity.DSC_ANALISIS = (reader["DSC_ANALISIS"] == DBNull.Value) ? String.Empty : reader["DSC_ANALISIS"].ToString();
                                //entity.DSC_OBSERVACION = (reader["DSC_OBSERVACION"] == DBNull.Value) ? String.Empty : reader["DSC_OBSERVACION"].ToString();
                                //entity.DSC_BALANZA = (reader["DSC_BALANZA"] == DBNull.Value) ? String.Empty : reader["DSC_BALANZA"].ToString();

                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;
                response.footerTable = Lista_Footer;
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
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

        public Response GetInventario(string idEmpresa, int Almacen, string CodInventario, string CodEstado, string flg_filtroFecha, string fch_inicio, string fch_fin)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            //ProductoBE entity = new ProductoBE();
            DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_FIND", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@ID_EMPRESA", SqlDbType.VarChar).Value = idEmpresa;
                        comando.Parameters.Add("@ID_ALMACEN", SqlDbType.Int).Value = DBNull.Value;
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = CodInventario;
                        comando.Parameters.Add("@COD_ESTADO", SqlDbType.VarChar).Value = CodEstado;
                        comando.Parameters.Add("@FLG_FILTRA_FECHA", SqlDbType.VarChar).Value = flg_filtroFecha;
                        comando.Parameters.Add("@FCH_INICIO", SqlDbType.VarChar).Value = fch_inicio;
                        comando.Parameters.Add("@FCH_FIN", SqlDbType.VarChar).Value = fch_fin;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            inventarioDataTable.Load(reader);
                            //while (reader.Read())
                            //{
                            //    entity.idProducto = (reader["ID_PRODUCTO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_PRODUCTO"].ToString());
                            //    entity.vchCodProducto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                            //    entity.vchDescripcion = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                            //    entity.intActivo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                            //    response.Entity = entity;
                            //}
                        }
                        response.Entity = inventarioDataTable;
                    }
                }

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
        public Response CerrarIventario(string codInventario, int conteo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_CerrarInventario_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codInventario", SqlDbType.VarChar, 20).Value = codInventario;
                        cmd.Parameters.Add("@conteo", SqlDbType.Int).Value = conteo;

                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        con.Open();
                        cmd.ExecuteReader();

                        response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) reader.Dispose();
            }
            return response;
        }
        public Response ConteoDiferencial(string codInventario, int conteo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_ConteoDiferencial_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codInventario", SqlDbType.VarChar, 20).Value = codInventario;
                        cmd.Parameters.Add("@conteo", SqlDbType.Int).Value = conteo;

                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        con.Open();
                        cmd.ExecuteReader();

                        response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) reader.Dispose();
            }
            return response;
        }
        public Response ConteoReinicio(string codInventario, int conteo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_ConteoReinicio_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codInventario", SqlDbType.VarChar, 20).Value = codInventario;
                        cmd.Parameters.Add("@conteo", SqlDbType.Int).Value = conteo;

                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        con.Open();
                        cmd.ExecuteReader();

                        response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) reader.Dispose();
            }
            return response;
        }

        public List<UsuarioBE> ListarUsuariosAsociados(string dscAlmacen)
        {
            List<UsuarioBE> Lista_result = new List<UsuarioBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarUsuariosAsociados_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@Dsc_Almacen", SqlDbType.VarChar, 50).Value = dscAlmacen;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioBE entity = new UsuarioBE();
                                entity.IdUsuario = (reader["ID_USUARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_USUARIO"].ToString());
                                entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
                                entity.Perfil = (reader["Perfil"] == DBNull.Value) ? String.Empty : reader["Perfil"].ToString();
                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                //response.Entity = Lista_result;
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return Lista_result;

        }

        public Response ExportInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3)
        {
            DataSet ds = new DataSet();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_GET_DETALLE_2", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = COD_INVENTARIO;
                        comando.Parameters.Add("@NRO_CONTEO_UNO", SqlDbType.Int).Value = NRO_CONTEO_1;
                        comando.Parameters.Add("@NRO_CONTEO_DOS", SqlDbType.Int).Value = NRO_CONTEO_2;
                        comando.Parameters.Add("@NRO_CONTEO_TRES", SqlDbType.Int).Value = NRO_CONTEO_3;


                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {

                            reader.NextResult();
                            DataTable Table0 = new DataTable("Tabla0");
                            Table0.Load(reader);
                            ds.Tables.Add(Table0);
                            //if (reader.NextResult())
                            //{
                            //    DataTable Table1 = new DataTable("Tabla1");
                            //    Table1.Load(reader);
                            //    ds.Tables.Add(Table0);
                            //    ds.Tables.Add(Table1);
                            //}


                        }
                        _ = new DataTable();
                        DataTable dt = ds.Tables[0];
                        response.Entity = dt;


                    }

                }


            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
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
        public Response DrawnBarChart(string CodInventario, int tipoGrafico)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_INVENTARIO_GET_GRAFICOS", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@TIPO_GRAFICO", SqlDbType.Int).Value = tipoGrafico;
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = CodInventario;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        { inventarioDataTable.Load(reader); }
                        response.Entity = inventarioDataTable;
                    }
                }

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

        public Response DrawnBarChart2(string CodInventario)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_GET_GRAFICOS", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@TIPO_GRAFICO", SqlDbType.Int).Value = 0;
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = CodInventario;
                        comando.Parameters.Add("@FILTRO", SqlDbType.VarChar).Value = "";
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        { inventarioDataTable.Load(reader); }
                        response.Entity = inventarioDataTable;
                    }
                }

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

        public Response ReportesInventario_WEB(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario,
            string start, string length, string order, string search)
        {
            Response response = new Response();
            List<decimal> Lista_Footer = new List<decimal>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_GET_REPORTE_2", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@TIPO_REPORTE", SqlDbType.Int).Value = tipoReporte;
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = Cod_inventario;
                        comando.Parameters.Add("@DATO_FILTRO", SqlDbType.VarChar).Value = DatoFiltro;
                        comando.Parameters.Add("@NROCONTEO", SqlDbType.VarChar).Value = NConteo;
                        comando.Parameters.Add("@USUARIO", SqlDbType.VarChar).Value = Usuario;

                        comando.Parameters.Add("@P_IDSTART", SqlDbType.VarChar).Value = start;
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.VarChar).Value = length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar).Value = order;
                        comando.Parameters.Add("@P_Search", SqlDbType.VarChar).Value = search;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());
                                if (tipoReporte == 0)
                                {
                                    Lista_Footer.Add((reader["Cant_Inicial"] == DBNull.Value) ? 0 : decimal.Parse(reader["Cant_Inicial"].ToString()));
                                    Lista_Footer.Add((reader["Inventariado"] == DBNull.Value) ? 0 : decimal.Parse(reader["Inventariado"].ToString()));
                                    Lista_Footer.Add((reader["Diferencial"] == DBNull.Value) ? 0 : decimal.Parse(reader["Diferencial"].ToString()));
                                }
                                if (tipoReporte == 1)
                                { Lista_Footer.Add((reader["STOCK_INVENTARIADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INVENTARIADO"].ToString())); }
                                if (tipoReporte == 2)
                                { Lista_Footer.Add((reader["STOCK_INVENTARIADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INVENTARIADO"].ToString())); }

                                if (tipoReporte == 3)
                                { Lista_Footer.Add((reader["STOCK_INVENTARIADO"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_INVENTARIADO"].ToString())); }
                            }
                            reader.NextResult();

                            inventarioDataTable.Load(reader);

                        }
                        response.Entity = inventarioDataTable;
                        response.footerTable = Lista_Footer;

                    }
                }

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
        public Response ReportesInventario(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_INVENTARIO_GET_REPORTE", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@TIPO_REPORTE", SqlDbType.Int).Value = tipoReporte;
                        comando.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar).Value = Cod_inventario;
                        comando.Parameters.Add("@DATO_FILTRO", SqlDbType.VarChar).Value = DatoFiltro;
                        comando.Parameters.Add("@NROCONTEO", SqlDbType.VarChar).Value = NConteo;
                        comando.Parameters.Add("@USUARIO", SqlDbType.VarChar).Value = Usuario;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        { inventarioDataTable.Load(reader); }
                        response.Entity = inventarioDataTable;
                    }
                }

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
        public Response ValidarDatosInventario(string Cod_Inv, int id_almacen)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            //DataTable inventarioDataTable = new DataTable();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ValidarDatosInv_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@Cod_Inv", SqlDbType.VarChar, 20).Value = Cod_Inv;
                        comando.Parameters.Add("@id_almacen", SqlDbType.Int).Value = id_almacen;
                        comando.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        comando.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        conexion.Open();

                        comando.ExecuteReader();

                        response.MENSAJE_ERROR = (string)comando.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)comando.Parameters["@Hubo_error"].Value;

                    }
                }

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

        public int ContarInventarios()
        {
            SqlCommand cmd = null;
            SqlConnection con = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("sp_ContarInventariosPreparados_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        object resultado = cmd.ExecuteScalar();

                        return resultado != null ? Convert.ToInt32(resultado) : 0;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }


        // YA NO ENVIA string xmlData
        public Response InsertInv_InvDetalle(Guid importacionId, string UserReg, string CodInv, int Id_almacen)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_CrearInventario_AND_Detalle_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        //comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        //ENVIAR importacionId

                        comando.Parameters.Add("@importacionId", SqlDbType.UniqueIdentifier).Value = importacionId;
                        comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;
                        comando.Parameters.Add("@CodInv", SqlDbType.VarChar, 20).Value = CodInv;
                        comando.Parameters.Add("@Id_Almacen", SqlDbType.Int).Value = Id_almacen;
                        comando.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        comando.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        conexion.Open();

                        comando.ExecuteReader();

                        response.MENSAJE_ERROR = (string)comando.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)comando.Parameters["@Hubo_error"].Value;


                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
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


        // AHORA SE ENVIA USERREG
        public List<DetInventarioImportBE> ImportarDetalles(XmlDocument xml_import, int IdAlmacen, string UserReg, Guid importacionId)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<DetInventarioImportBE> Lista_result = new List<DetInventarioImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportDetallesXML_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString(); // REVISAR ESE TOSTRING YA QUE INNERXML YA ES UN STRING
                        comando.Parameters.Add("@IDAlmacen", SqlDbType.Int).Value = IdAlmacen;
                        comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = UserReg;
                        comando.Parameters.Add("@importacionId", SqlDbType.UniqueIdentifier).Value = importacionId;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //ProductoBE entity = new ProductoBE();
                                DetInventarioImportBE Importobj = new DetInventarioImportBE();
                                Importobj.COD_UBICACION = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                Importobj.DSC_UBICACION = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();
                                Importobj.COD_PRODUCTO = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                Importobj.DSC_PRODUCTO = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                Importobj.importacionId = Guid.Parse(reader["ImportacionId"].ToString());
                                Importobj.LOTE_PRODUCTO = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                Importobj.STOCK_ACTUAL = (reader["STOCK_ACTUAL"] == DBNull.Value) ? 0 : decimal.Parse(reader["STOCK_ACTUAL"].ToString());
                                Importobj.Flg_Pass = (reader["Flg_Pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["Flg_Pass"].ToString());
                                Importobj.Desc_Error = (reader["Desc_Error"] == DBNull.Value) ? String.Empty : reader["Desc_Error"].ToString();
                                Lista_result.Add(Importobj);
                            }
                        }
                    }
                }
                //response.Entity = Lista_result;
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return Lista_result;

        }

        // MODIFICAR STOCK DE LA TABLA TEMPORAL DE LA DB
        public Response ModificarProductoStock(Guid importacionId, string codUbicacion, string codProducto, string lote, double stock)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_MantCampoTablaDetalle_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@ImportacionId ", SqlDbType.UniqueIdentifier).Value = importacionId;
                        comando.Parameters.Add("@CodUbicacion", SqlDbType.VarChar, 20).Value = codUbicacion;
                        comando.Parameters.Add("@CodProducto", SqlDbType.VarChar, 50).Value = codProducto;
                        comando.Parameters.Add("@LoteProducto", SqlDbType.VarChar, 20).Value = lote;
                        comando.Parameters.Add("@StockActual", SqlDbType.Decimal).Value = Convert.ToDecimal(stock);
                        comando.Parameters.Add("@HUBO_ERROR", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        comando.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        conexion.Open();

                        comando.ExecuteReader();

                        response.MENSAJE_ERROR = (string)comando.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)comando.Parameters["@Hubo_error"].Value;


                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
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


        public Response GetAPI_StockALM()
        {
            //string Stringresult;
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("GET_APIEXTERNA_STOCKALM", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        comando.Parameters.Add("@msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                        conexion.Open();
                        comando.ExecuteReader();
                        response.MENSAJE_ERROR = (string)comando.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)comando.Parameters["@Hubo_error"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
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

        public int? CLI_SP_CREAR_INVENTARIO(string COD_ALMACEN, out bool huboError, out string mensaje)
        {
            int? idInventario = null;
            huboError = false;
            mensaje = "";

            using (SqlConnection connection = new SqlConnection(Connection.AppStringConection()))
            {
                using (SqlCommand command = new SqlCommand("CLI_SP_CREAR_INVENTARIO", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    command.Parameters.Add("@COD_ALMACEN", SqlDbType.VarChar, 50).Value = COD_ALMACEN ?? (object)DBNull.Value;
                    //command.Parameters.Add("@CLISTA_DETALLE", SqlDbType.Xml).Value = CLISTA_DETALLE_XML ?? (object)DBNull.Value;

                    // Parámetros de salida
                    SqlParameter huboErrorParameter = command.Parameters.Add("@Hubo_error", SqlDbType.Bit);
                    huboErrorParameter.Direction = ParameterDirection.Output;

                    SqlParameter mensajeParameter = command.Parameters.Add("@msg", SqlDbType.VarChar, 200);
                    mensajeParameter.Direction = ParameterDirection.Output;

                    // Parámetro de retorno
                    SqlParameter returnValueParameter = command.Parameters.Add("@ID_INVENTARIO", SqlDbType.Int);
                    returnValueParameter.Direction = ParameterDirection.Output;

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Obtener valores de salida
                        huboError = (bool)huboErrorParameter.Value;
                        mensaje = mensajeParameter.Value as string;
                        idInventario = returnValueParameter.Value as int?;
                    }
                    catch (Exception ex)
                    {
                        huboError = true;
                        mensaje = "Ocurrió un error: " + ex.Message;
                    }
                }
            }

            return idInventario;
        }

        public void ImportarDetInventario_Movil(XmlDocument xml_import, string UserReg, int intIdInventario)
        {
            using (SqlConnection connection = new SqlConnection(Connection.AppStringConection()))
            {
                using (SqlCommand command = new SqlCommand("WEB_ImportarDetalleInventario_Movil_2024", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada

                    command.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml).Value = xml_import.InnerXml.ToString();
                    command.Parameters.Add("@intIdInventario", SqlDbType.Int).Value = intIdInventario;
                    command.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg ?? (object)DBNull.Value;
                    command.Parameters.Add("@Pass", SqlDbType.Int).Value = 1;
                    connection.Open();
                    command.ExecuteNonQuery();


                }
            }

        }

        public void Insert_ASF_DETALLE_INVENTARIO(string UserReg)
        {
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarDetalleInventarioFINAL_Inventario_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;

                        conexion.Open();
                        comando.ExecuteNonQuery();

                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }


        }
    }
}
