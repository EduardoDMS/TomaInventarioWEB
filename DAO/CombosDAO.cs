using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class CombosDAO
    {
        public Response cbxAlmacenes()
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxAlmacenes_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                entity.intValue = (reader["ID_ALMACEN"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_ALMACEN"].ToString());
                                entity.vchdesc = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
                                entity.vchValue = (reader["COD_ALMACEN"] == DBNull.Value) ? String.Empty : reader["COD_ALMACEN"].ToString();
                                ListCombo.Add(entity);
                            }
                        }

                    }

                }
                response.Entity = ListCombo;
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

        public Response cbxInventario()
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxInventario_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                entity.vchValue = (reader["ID_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["ID_INVENTARIO"].ToString();
                                entity.vchdesc = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
                                ListCombo.Add(entity);
                            }
                        }

                    }

                }
                response.Entity = ListCombo;
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

        public Response cbxInventariosPorAlmacen(string idAlmacen)
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxInventariosCerrados", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@COD_ALMACEN", SqlDbType.VarChar).Value = idAlmacen;// es codAlmacen pero se identifica como idAlmacen
                                                                                                // cmd.Parameters.Clear();
                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                entity.vchValue = (reader["ID_INVENTARIO"] == DBNull.Value) ?
                                    String.Empty : reader["ID_INVENTARIO"].ToString();
                                entity.vchdesc = (reader["COD_INVENTARIO"] == DBNull.Value) ?
                                    String.Empty : reader["COD_INVENTARIO"].ToString();
                                ListCombo.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = ListCombo;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }

            return response;
        }

        public Response cbxUbicacion(int id_almacen)
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxUbicacion_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Id_Almacen", SqlDbType.Int).Value = id_almacen;

                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                //entity.intValue = (reader["ID_UBICACION"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_UBICACION"].ToString());
                                entity.vchValue = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.vchdesc = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();
                                ListCombo.Add(entity);
                            }
                        }

                    }

                }
                response.Entity = ListCombo;
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

        public Response cbxProducto(string dsc_prod)
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxProducto_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@DSC_PROD", SqlDbType.VarChar, 200).Value = dsc_prod;

                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                entity.vchValue = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.vchdesc = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.intaddValue = (reader["intUM"] == DBNull.Value) ? 0 : Int32.Parse(reader["intUM"].ToString());
                                ListCombo.Add(entity);
                            }
                        }

                    }

                }
                response.Entity = ListCombo;
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

        //public Response cbxInventariosCerrados()
        //{
        //    Response response = new Response();
        //    List<CombosBE> ListCombo = new List<CombosBE>();
        //    SqlConnection con = null;
        //    SqlCommand cmd = null;
        //    SqlDataReader reader = null;
        //    try
        //    {
        //        using (con = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (cmd = new SqlCommand("WEB_cbxInventariosCerrados_2024", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Clear();
        //                con.Open();
        //                using (reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        CombosBE entity = new CombosBE();
        //                        //entity.intValue = (reader["ID_INVENTARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_INVENTARIO"].ToString());
        //                        entity.vchdesc = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
        //                        ListCombo.Add(entity);
        //                    }
        //                }

        //            }

        //        }
        //        response.Entity = ListCombo;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.MENSAJE_ERROR = ex.Message.ToString();
        //        response.HUBO_ERROR = true;

        //    }
        //    finally
        //    {
        //        if (con != null) { con.Close(); con.Dispose(); }
        //        //if (con != null) { con.Dispose(); }
        //        if (reader != null) { reader.Dispose(); }
        //    }
        //    return response;
        //}

        /// <summary>
        /// Obtiene lista de inventarios cerrados para combo
        /// </summary>
        public Response cbxInventariosCerrados()
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_cbxInventariosCerrados", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Add("@ID_ALMACEN", SqlDbType.Int).Value = NRO_CONTEO_3;
                        cmd.Parameters.Clear();
                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();
                                entity.vchValue = (reader["ID_INVENTARIO"] == DBNull.Value) ?
                                    String.Empty : reader["ID_INVENTARIO"].ToString();
                                entity.vchdesc = (reader["COD_INVENTARIO"] == DBNull.Value) ?
                                    String.Empty : reader["COD_INVENTARIO"].ToString();
                                ListCombo.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = ListCombo;
            }
            catch (Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
            }

            return response;
        }

        public Response cbxUM()
        {
            Response response = new Response();
            List<CombosBE> ListCombo = new List<CombosBE>();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_CbxUnidadMedida_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        con.Open();
                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CombosBE entity = new CombosBE();

                                entity.intValue = (reader["IntUniMed"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntUniMed"].ToString());
                                entity.vchdesc = (reader["vchDesUniMed"] == DBNull.Value) ? String.Empty : reader["vchDesUniMed"].ToString();
                                ListCombo.Add(entity);
                            }
                        }

                    }

                }
                response.Entity = ListCombo;
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
