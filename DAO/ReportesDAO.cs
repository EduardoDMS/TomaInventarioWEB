using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections;

namespace DAO
{
    public class ReportesDAO
    {
        public Response TablaHistorico(int ID_Almacen,string filtrarFecha,string dtmIni, string dtmFin)
        {
            Response response = new Response();
            
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            DataTable dtHistorico = new DataTable();

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("ASF_SP_INVENTARIO_FIND", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ID_EMPRESA", SqlDbType.VarChar,-1).Value = "1";
                        cmd.Parameters.Add("@ID_ALMACEN", SqlDbType.Int).Value = ID_Almacen;
                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar,20).Value = "";
                        cmd.Parameters.Add("@COD_ESTADO", SqlDbType.VarChar,3).Value = DBNull.Value;
                        cmd.Parameters.Add("@FLG_FILTRA_FECHA", SqlDbType.VarChar,1).Value = filtrarFecha;
                        cmd.Parameters.Add("@FCH_INICIO", SqlDbType.VarChar,50).Value = dtmIni;
                        cmd.Parameters.Add("@FCH_FIN", SqlDbType.VarChar,50).Value = dtmFin;
                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            dtHistorico.Load(reader);
                            response.Entity = dtHistorico;
                        }
                    }
                }
                //response.Entity = ListCombo;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;

            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                //if (con != null) { con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }
            return response;
        }
        public Response TablaDetalle_WEB(string cod_Inventario, int NConteo, string start, string length, string order, string search)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("ASF_SP_INVENTARIO_GET_DETALLE_PAGINADO", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = cod_Inventario;
                        cmd.Parameters.Add("@COD_UBICACION", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@LOTE_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_USUARIO_REGISTRO", SqlDbType.VarChar, 20).Value = "";
                        cmd.Parameters.Add("@ID_TERMINAL", SqlDbType.VarChar, 10).Value = "";
                        cmd.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = NConteo;

                        cmd.Parameters.Add("@P_IDSTART", SqlDbType.VarChar, 20).Value = start;
                        cmd.Parameters.Add("@P_LENGTH", SqlDbType.VarChar, 20).Value = length;
                        cmd.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 20).Value = order;
                        cmd.Parameters.Add("@P_Search", SqlDbType.VarChar, 500).Value = search;

                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());
                            }
                            reader.NextResult();
                            dt.Load(reader);
                          
                        }
                        response.Entity = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;

            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                //if (con != null) { con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }
            return response;
        }
        public Response TablaDetalle(string cod_Inventario, int NConteo)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("ASF_SP_INVENTARIO_GET_DETALLE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = cod_Inventario;
                        cmd.Parameters.Add("@COD_UBICACION", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@LOTE_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_USUARIO_REGISTRO", SqlDbType.VarChar, 20).Value = "";
                        cmd.Parameters.Add("@ID_TERMINAL", SqlDbType.VarChar, 10).Value = "";
                        cmd.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = NConteo;
                        
                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                            response.Entity = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;

            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                //if (con != null) { con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }
            return response;
        }
        public Response TablaLecturas(string cod_Inventario, int NConteo)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            DataTable dt = new DataTable();

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("ASF_SP_INVENTARIO_GET_LECTURA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = cod_Inventario;
                        cmd.Parameters.Add("@COD_UBICACION", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@LOTE_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@SERIE_PRODUCTO", SqlDbType.VarChar, 50).Value = "";
                        cmd.Parameters.Add("@COD_USUARIO_REGISTRO", SqlDbType.VarChar, 20).Value = "";
                        cmd.Parameters.Add("@ID_TERMINAL", SqlDbType.VarChar, 10).Value = "";
                        cmd.Parameters.Add("@NRO_CONTEO", SqlDbType.Int).Value = NConteo;

                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                            response.Entity = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;

            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                //if (con != null) { con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }
            return response;
        }
    }
}
