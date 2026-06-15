using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace DAO
{
    public class MantenimientosDAO
    {
        #region Mant Usuarios
        public List<UsuarioBE> ListarUsuarios(string perfil, string usuario)
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
                    using (comando = new SqlCommand("WEB_ListarUsuarios_2024", conexion)) // aun sirve este procedure
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@perfil", SqlDbType.VarChar, 20).Value = perfil;
                        comando.Parameters.Add("@user", SqlDbType.VarChar, 20).Value = usuario;


                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioBE entity = new UsuarioBE();
                                entity.IdUsuario = (reader["ID_USUARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_USUARIO"].ToString());
                                entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
                                entity.Perfil = (reader["Perfil"] == DBNull.Value) ? String.Empty : reader["Perfil"].ToString();
                                entity.vchActivo = (reader["Estado"] == DBNull.Value) ? String.Empty : reader["Estado"].ToString();

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
        public Response CreateUsuario(string cod_Usuario, string nombre, string apellido, string clave, string perfil, string idAlmacen)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_createUsuarios_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@cod_usuario", SqlDbType.VarChar,50).Value = cod_Usuario; //agregado
                        cmd.Parameters.Add("@nombre_usuario", SqlDbType.VarChar, 200).Value = nombre;
                        cmd.Parameters.Add("@apellido_usuario", SqlDbType.VarChar, 200).Value = apellido;
                        cmd.Parameters.Add("@clave", SqlDbType.VarChar, 100).Value = clave;
                        cmd.Parameters.Add("@perfil", SqlDbType.VarChar, 10).Value = perfil;
                        cmd.Parameters.Add("@vchidAlmacen", SqlDbType.VarChar, -1).Value = idAlmacen;
                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;

                        foreach (SqlParameter p in cmd.Parameters)
                            Console.WriteLine($"{p.ParameterName} = {p.Value}");

                        con.Open();
                        cmd.ExecuteNonQuery();

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
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();

            }
            return response;
        }

        public Response GetUsuario(int idUsuario)
        {
            List<UsuarioBE> Lista_result = new List<UsuarioBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            //UsuarioBE entity = new UsuarioBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("Web_GetUsuario_2026", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioBE entity = new UsuarioBE();
                                entity.Nombre = (reader["NOMBRE_USUARIO"] == DBNull.Value) ? String.Empty : reader["NOMBRE_USUARIO"].ToString();
                                entity.Apellido = (reader["APELLIDO_USUARIO"] == DBNull.Value) ? String.Empty : reader["APELLIDO_USUARIO"].ToString();
                                entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
                                entity.Contraseña = (reader["CLAVE_USUARIO"] == DBNull.Value) ? String.Empty : reader["CLAVE_USUARIO"].ToString();
                                entity.Perfil = (reader["PERFIL_USUARIO"] == DBNull.Value) ? String.Empty : reader["PERFIL_USUARIO"].ToString();
                                entity.IdAlmacen = (reader["ID_ALMACEN"] == DBNull.Value) ? -1 : Int32.Parse(reader["ID_ALMACEN"].ToString());
                                entity.Activo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());

                                //response.Entity = entity;
                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;

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
        public Response UpdateUsuario(int idUsuario, string cod_usuario, string apellidoUsuario, string nombreUsuario, string clave, string perfil, string idAlmacen, bool activo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_updateUsuario_2026", con)) // WEB_updateUsuario_2024
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear(); // Ver el tema de los tamaños existe una incongruencia  
                        cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
                        cmd.Parameters.Add("@cod_usuario", SqlDbType.VarChar,20).Value = cod_usuario;
                        cmd.Parameters.Add("@apellido_usuario", SqlDbType.VarChar, 20).Value = apellidoUsuario;// se agrego este campo de apellido para que se pueda actualizar el apellido del usuario, se agrego en el stored procedure y en el metodo createUsuario
                        cmd.Parameters.Add("@nombre_usuario", SqlDbType.VarChar, 20).Value = nombreUsuario;// se modifico este campo de nombre a usuario para que sea mas entendible
                        cmd.Parameters.Add("@clave", SqlDbType.VarChar, 20).Value = clave;
                        cmd.Parameters.Add("@perfil", SqlDbType.VarChar, 3).Value = perfil;
                        //cmd.Parameters.Add("@idAlmacen", SqlDbType.Int).Value = idAlmacen;
                        cmd.Parameters.Add("@vchidAlmacen", SqlDbType.VarChar, -1).Value = idAlmacen;

                        cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;
                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        con.Open();
                        cmd.ExecuteNonQuery();

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

        #endregion

        #region Mant Almacenes
        public List<AlmacenBE> ListarAlmacenes(string dscAlmacen, string activo)
        {
            List<AlmacenBE> Lista_result = new List<AlmacenBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarAlmacenes_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@dscAlmacen", SqlDbType.VarChar, 200).Value = dscAlmacen;
                        comando.Parameters.Add("@activo", SqlDbType.VarChar, 1).Value = activo;


                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlmacenBE entity = new AlmacenBE();
                                entity.idAlmacen = (reader["ID_ALMACEN"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_ALMACEN"].ToString());
                                entity.vchcodAlmacen = (reader["COD_ALMACEN"] == DBNull.Value) ? String.Empty : reader["COD_ALMACEN"].ToString();
                                entity.vchdscAlmacen = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
                                entity.vchActivo = (reader["Estado"] == DBNull.Value) ? String.Empty : reader["Estado"].ToString();
                                entity.vchTipoAlmacen = (reader["TIPO_ALMACEN"] == DBNull.Value) ? String.Empty : reader["TIPO_ALMACEN"].ToString();
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
        public Response CreateAlmacen(string codAlmacen, string descAlmacen)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_createAlmacenes_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codAlmacen", SqlDbType.VarChar, 20).Value = codAlmacen;
                        cmd.Parameters.Add("@dscAlmacen", SqlDbType.VarChar, 200).Value = descAlmacen;

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
        public Response GetAlmacen(int idAlmacen)
        {
            //List<UsuarioBE> Lista_result = new List<UsuarioBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            AlmacenBE entity = new AlmacenBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_GetAlmacen_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@idAlmacen", SqlDbType.Int).Value = idAlmacen;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.idAlmacen = (reader["ID_ALMACEN"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_ALMACEN"].ToString());
                                entity.vchcodAlmacen = (reader["COD_ALMACEN"] == DBNull.Value) ? String.Empty : reader["COD_ALMACEN"].ToString();
                                entity.vchdscAlmacen = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
                                entity.intActivo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                                entity.vchTipoAlmacen = (reader["TIPO_ALMACEN"] == DBNull.Value) ? String.Empty : reader["TIPO_ALMACEN"].ToString();
                                response.Entity = entity;
                            }
                        }
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
        public Response UpdateAlmacen(int idAlmacen, string codAlmacen, string dscAlmacen, bool activo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_updateAlmacen_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@idAlmacen", SqlDbType.Int).Value = idAlmacen;
                        cmd.Parameters.Add("@codAlmacen", SqlDbType.VarChar, 20).Value = codAlmacen;
                        cmd.Parameters.Add("@DSC_Almacen", SqlDbType.VarChar, 200).Value = dscAlmacen;
                        cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

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
        public Response GetAPIAlmacen()
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
                    using (comando = new SqlCommand("GET_APIEXTERNA_Almacenes", conexion))
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

        public Response CargarAlmacenesAPIExterna(AlmacenAPI Almacen)
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
                    using (comando = new SqlCommand("WEB_InsertAlmacenFromAPI_2025", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        //comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml).Value = xml_import.InnerXml.ToString();

                        comando.Parameters.Add("@CodAlmacen", SqlDbType.VarChar, 20).Value = Almacen.ALMACEN_CODIGO;
                        comando.Parameters.Add("@DSCAlmacen", SqlDbType.VarChar, 200).Value = Almacen.ALMACEN_NOMBRE;
                        comando.Parameters.Add("@CodUbicacion", SqlDbType.VarChar, 20).Value = Almacen.UBICACION_CODIGO;
                        comando.Parameters.Add("@DSCUbicacion", SqlDbType.VarChar, 200).Value = Almacen.UBICACION_NOMBRE;

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


        #endregion

        #region Mant Ubicacion
        public Response ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen, string start, string length, string order)
        {
            List<UbicacionBE> Lista_result = new List<UbicacionBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarUbicaciones_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@activo", SqlDbType.VarChar, 1).Value = activo;
                        comando.Parameters.Add("@dscUbicacion", SqlDbType.VarChar, 20).Value = vchUbicacion;
                        comando.Parameters.Add("@IdALmacen", SqlDbType.Int).Value = idAlmacen;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.VarChar, 20).Value = start;
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.VarChar, 20).Value = length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 20).Value = order;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                UbicacionBE entity = new UbicacionBE();
                                //entity.intIdUbicacion = (reader["ID_UBICACION"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_UBICACION"].ToString());
                                entity.vchCod_Ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.vchDSC_Ubicacion = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();
                                entity.vchDSC_Almacen = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
                                entity.vchActivo = (reader["Estado"] == DBNull.Value) ? String.Empty : reader["Estado"].ToString();

                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;
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


        //public List<UbicacionBE> ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen)
        //{
        //    List<UbicacionBE> Lista_result = new List<UbicacionBE>();
        //    Response response = new Response();
        //    SqlConnection conexion = null;
        //    SqlCommand comando = null;
        //    SqlDataReader reader = null;

        //    try
        //    {
        //        using (conexion = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (comando = new SqlCommand("WEB_ListarUbicaciones_2024", conexion))
        //            {
        //                comando.CommandType = CommandType.StoredProcedure;
        //                comando.Parameters.Clear();
        //                comando.Parameters.Add("@activo", SqlDbType.VarChar, 1).Value = activo;
        //                comando.Parameters.Add("@dscUbicacion", SqlDbType.VarChar, 20).Value = vchUbicacion;
        //                comando.Parameters.Add("@IdALmacen", SqlDbType.Int).Value = idAlmacen;
        //                conexion.Open();

        //                using (reader = comando.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        UbicacionBE entity = new UbicacionBE();
        //                        //entity.intIdUbicacion = (reader["ID_UBICACION"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_UBICACION"].ToString());
        //                        entity.vchCod_Ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
        //                        entity.vchDSC_Ubicacion = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();
        //                        entity.vchDSC_Almacen = (reader["DSC_ALMACEN"] == DBNull.Value) ? String.Empty : reader["DSC_ALMACEN"].ToString();
        //                        entity.vchActivo = (reader["Estado"] == DBNull.Value) ? String.Empty : reader["Estado"].ToString();

        //                        Lista_result.Add(entity);
        //                    }
        //                }
        //            }
        //        }
        //        //response.Entity = Lista_result;
        //    }
        //    catch (Exception e)
        //    {
        //        response.MENSAJE_ERROR = e.Message.ToString();
        //        response.HUBO_ERROR = true;
        //    }
        //    finally
        //    {
        //        if (conexion != null) { conexion.Close(); conexion.Dispose(); }
        //        if (comando != null) comando.Dispose();
        //        if (reader != null) reader.Dispose();
        //    }

        //    return Lista_result;

        //}
        public Response CreateUbicacion(string codUbicacion, string dscUbicacion, string idAlmacen)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_createUbicaciones_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codUbicacion", SqlDbType.VarChar, 20).Value = codUbicacion;
                        cmd.Parameters.Add("@dscUbicacion", SqlDbType.VarChar, 200).Value = dscUbicacion;
                        cmd.Parameters.Add("@vchidAlmacen", SqlDbType.VarChar, -1).Value = idAlmacen;

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
        public Response GetUbicacion(string CodUbicacion)
        {
            List<UbicacionBE> Lista_result = new List<UbicacionBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_GetUbicacion_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@codUbicacion", SqlDbType.VarChar, 20).Value = CodUbicacion;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UbicacionBE entity = new UbicacionBE();
                                entity.vchCod_Ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.vchDSC_Ubicacion = (reader["DSC_UBICACION"] == DBNull.Value) ? String.Empty : reader["DSC_UBICACION"].ToString();
                                entity.intActivo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                                entity.intIdAlmacen = (reader["ID_ALMACEN"] == DBNull.Value) ? -1 : Int32.Parse(reader["ID_ALMACEN"].ToString());

                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;

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
        public Response UpdateUbicacion(string codUbicacion, string DSCUbicacion, string idAlmacen, bool activo)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_updateUbicacion_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codUbicacion", SqlDbType.VarChar, 20).Value = codUbicacion;
                        cmd.Parameters.Add("@DSC_Ubicacion", SqlDbType.VarChar, 200).Value = DSCUbicacion;
                        cmd.Parameters.Add("@vchidAlmacen", SqlDbType.VarChar, -1).Value = idAlmacen;
                        cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

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

        #endregion

        #region Mant Productos
        public Response ListarProductos(int activo, string vchProducto, string start, string length, string order)
        {
            List<ProductoBE> Lista_result = new List<ProductoBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarProductos_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@vchFiltro", SqlDbType.VarChar, 200).Value = vchProducto;
                        comando.Parameters.Add("@activo", SqlDbType.Int).Value = activo;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.VarChar, 20).Value = start;
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.VarChar, 20).Value = length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 20).Value = order;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                ProductoBE entity = new ProductoBE();
                                entity.idProducto = (reader["ID_PRODUCTO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_PRODUCTO"].ToString());
                                entity.vchCodProducto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.vchDescripcion = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.intActivo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                                entity.vchActivo = (reader["vchActivo"] == DBNull.Value) ? String.Empty : reader["vchActivo"].ToString();
                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;
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
        public Response CreateProducto(string codProducto, string descProducto, int UM)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_createProducto_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@codProducto", SqlDbType.VarChar, 20).Value = codProducto;
                        cmd.Parameters.Add("@dscProducto", SqlDbType.VarChar, 200).Value = descProducto;
                        cmd.Parameters.Add("@UM", SqlDbType.Int).Value = UM;

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
        public Response GetProducto(int id)
        {
            //List<UsuarioBE> Lista_result = new List<UsuarioBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            ProductoBE entity = new ProductoBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_GetProducto_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@idProducto", SqlDbType.Int).Value = id;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.idProducto = (reader["ID_PRODUCTO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_PRODUCTO"].ToString());
                                entity.vchCodProducto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.vchDescripcion = (reader["DSC_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["DSC_PRODUCTO"].ToString();
                                entity.intActivo = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                                entity.intUM = (reader["IntUniMed"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntUniMed"].ToString());
                                response.Entity = entity;
                            }
                        }
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
        public Response UpdateProducto(int id, string codProducto, string descProducto, string usuario, bool activo, int UM)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_updateProducto_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@idProducto", SqlDbType.Int).Value = id;
                        cmd.Parameters.Add("@codProducto", SqlDbType.VarChar, 20).Value = codProducto;
                        cmd.Parameters.Add("@DSC_Producto", SqlDbType.VarChar, 200).Value = descProducto;
                        cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario;
                        cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;
                        cmd.Parameters.Add("@UM", SqlDbType.Int).Value = UM;

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

        public Response GetAPIProductos()
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
                    using (comando = new SqlCommand("GET_APIEXTERNA_PRODUCTOS", conexion))
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

        public Response CargarProductosAPIExterna(ProductoBEAPI producto)
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
                    using (comando = new SqlCommand("WEB_InsertProductoFromAPI_2025", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        //comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml).Value = xml_import.InnerXml.ToString();

                        comando.Parameters.Add("@CodProducto", SqlDbType.VarChar, 50).Value = producto.PRODUCTO_CODIGO;
                        comando.Parameters.Add("@DSCProducto", SqlDbType.VarChar, 200).Value = producto.PRODUCTO_NOMBRE;
                        comando.Parameters.Add("@CODUM", SqlDbType.VarChar, 100).Value = producto.UNIDADMEDIDA_CODIGO;


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


        public List<ImportBE> ImportarProductos(XmlDocument xml_import)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportProductosXML_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductoBE entity = new ProductoBE();
                                ImportBE Importobj = new ImportBE();
                                entity.vchCodProducto = (reader["Cod_Producto"] == DBNull.Value) ? String.Empty : reader["Cod_Producto"].ToString();
                                entity.vchDescripcion = (reader["Desc_Producto"] == DBNull.Value) ? String.Empty : reader["Desc_Producto"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["Flg_Pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["Flg_Pass"].ToString());
                                Importobj.Mensaje = (reader["Desc_Error"] == DBNull.Value) ? String.Empty : reader["Desc_Error"].ToString();
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

        public List<ImportBE> ImportarUbicaciones(XmlDocument xml_import)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportUbicacionXML_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UbicacionBE entity = new UbicacionBE();
                                ImportBE Importobj = new ImportBE();
                                entity.vchCOD_Almacen = (reader["Cod_Almacen"] == DBNull.Value) ? String.Empty : reader["Cod_Almacen"].ToString();
                                entity.vchCod_Ubicacion = (reader["Cod_Ubicacion"] == DBNull.Value) ? String.Empty : reader["Cod_Ubicacion"].ToString();
                                entity.vchDSC_Ubicacion = (reader["Desc_Ubicacion"] == DBNull.Value) ? String.Empty : reader["Desc_Ubicacion"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["Flg_Pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["Flg_Pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        #endregion

        #region Unidad Medida
        public Response ListarUnidadMedida(string vchUnidadMedida, string start, string length, string order)
        {
            List<UnidadMedidaBE> Lista_result = new List<UnidadMedidaBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarUnidadMedida_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@vchFiltro", SqlDbType.VarChar, 20).Value = vchUnidadMedida;
                        comando.Parameters.Add("@P_IDSTART", SqlDbType.VarChar, 20).Value = start;
                        comando.Parameters.Add("@P_LENGTH", SqlDbType.VarChar, 20).Value = length;
                        comando.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 20).Value = order;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                response.count = (reader["filas_total"] == DBNull.Value) ? 0 : Int32.Parse(reader["filas_total"].ToString());
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                UnidadMedidaBE entity = new UnidadMedidaBE();
                                entity.IntUniMed = (reader["IntUniMed"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntUniMed"].ToString());
                                entity.vchCodUniMed = (reader["vchCodUniMed"] == DBNull.Value) ? String.Empty : reader["vchCodUniMed"].ToString();
                                entity.vchDesUniMed = (reader["vchDesUniMed"] == DBNull.Value) ? String.Empty : reader["vchDesUniMed"].ToString();

                                entity.IntEstado = (reader["IntEstado"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntEstado"].ToString());
                                entity.vchEstado = (reader["vchActivo"] == DBNull.Value) ? String.Empty : reader["vchActivo"].ToString();
                                Lista_result.Add(entity);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;
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
        public Response CreateUnidadMedida(UnidadMedidaBE obj)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_createUnidadMedida_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@cod", SqlDbType.VarChar, 20).Value = obj.vchCodUniMed;
                        cmd.Parameters.Add("@dsc", SqlDbType.VarChar, 20).Value = obj.vchDesUniMed;
                        cmd.Parameters.Add("@cantidad", SqlDbType.Int).Value = obj.IntCantidad;

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
        public Response GetUnidadMedida(int id)
        {
            //List<UsuarioBE> Lista_result = new List<UsuarioBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            UnidadMedidaBE entity = new UnidadMedidaBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_GetUnidadMedida_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.IntUniMed = (reader["IntUniMed"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntUniMed"].ToString());
                                entity.vchCodUniMed = (reader["vchCodUniMed"] == DBNull.Value) ? String.Empty : reader["vchCodUniMed"].ToString();
                                entity.vchDesUniMed = (reader["vchDesUniMed"] == DBNull.Value) ? String.Empty : reader["vchDesUniMed"].ToString();
                                entity.IntEstado = (reader["IntEstado"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntEstado"].ToString());
                                entity.IntCantidad = (reader["IntCantidad"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntCantidad"].ToString());
                                response.Entity = entity;
                            }
                        }
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
        public Response UpdateUnidadMedida(UnidadMedidaBE obj)
        {
            Response response = new Response();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_updateUnidadMedida_2024", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IntUniMed;
                        cmd.Parameters.Add("@dsc", SqlDbType.VarChar, 20).Value = obj.vchDesUniMed;
                        cmd.Parameters.Add("@cantidad", SqlDbType.VarChar, 200).Value = obj.IntCantidad;
                        cmd.Parameters.Add("@estado", SqlDbType.VarChar, 50).Value = obj.IntEstado;

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

        public List<UMEXCELBE> ListarUnidadMedidaEXCEL()
        {
            List<UMEXCELBE> Lista_result = new List<UMEXCELBE>();
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ListarUnidadMedidaEXCEL_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UMEXCELBE entity = new UMEXCELBE();
                                entity.vchCodUniMed = (reader["vchCodUniMed"] == DBNull.Value) ? String.Empty : reader["vchCodUniMed"].ToString();
                                entity.vchDesUniMed = (reader["vchDesUniMed"] == DBNull.Value) ? String.Empty : reader["vchDesUniMed"].ToString();
                                entity.IntCantidad = (reader["IntCantidad"] == DBNull.Value) ? 0 : Int32.Parse(reader["IntCantidad"].ToString());
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


        #endregion

        #region Importación Maestro
        public List<ImportBE> ImportarAlmacen_Maestro(XmlDocument xml_import)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarAlmacen_Maestro_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlmacenBE entity = new AlmacenBE();
                                ImportBE Importobj = new ImportBE();
                                entity.vchcodAlmacen = (reader["Cod_Almacen"] == DBNull.Value) ? String.Empty : reader["Cod_Almacen"].ToString();
                                entity.vchdscAlmacen = (reader["Desc_Almacen"] == DBNull.Value) ? String.Empty : reader["Desc_Almacen"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = 1;
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        public List<ImportBE> ImportarUsuarios_Maestro(XmlDocument xml_import, string UserReg)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarUsuarios_Maestro_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioBE entity = new UsuarioBE();
                                ImportBE Importobj = new ImportBE();
                                entity.Usuario = (reader["Cod_Usuario"] == DBNull.Value) ? String.Empty : reader["Cod_Usuario"].ToString();
                                entity.Perfil = (reader["Perfil_Usuario"] == DBNull.Value) ? String.Empty : reader["Perfil_Usuario"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        public List<ImportBE> ImportarUsuariosXAlmacen_Maestro(XmlDocument xml_import)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarUsuarioXAlmacen_Maestro_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioBE entity = new UsuarioBE();
                                ImportBE Importobj = new ImportBE();
                                entity.Usuario = (reader["Cod_Usuario"] == DBNull.Value) ? String.Empty : reader["Cod_Usuario"].ToString();
                                entity.Perfil = (reader["Cod_Almacen"] == DBNull.Value) ? String.Empty : reader["Cod_Almacen"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["Flg_Pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["Flg_Pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        public List<ImportBE> ImportarUbicacion_Maestro(XmlDocument xml_import)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarUbicacion_Maestro_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UbicacionBE entity = new UbicacionBE();
                                ImportBE Importobj = new ImportBE();
                                entity.vchCOD_Almacen = (reader["Cod_Almacen"] == DBNull.Value) ? String.Empty : reader["Cod_Almacen"].ToString();
                                entity.vchCod_Ubicacion = (reader["Cod_Ubicacion"] == DBNull.Value) ? String.Empty : reader["Cod_Ubicacion"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["Flg_Pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["Flg_Pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        #endregion

        #region Importación Inventario
        public List<ImportBE> ImportarInventario_Inventario(XmlDocument xml_import, string UserReg)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarInventario_Inventario_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;
                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InventarioBE entity = new InventarioBE();
                                ImportBE Importobj = new ImportBE();
                                entity.Cod_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
                                entity.Cod_Almacen = (reader["COD_ALMACEN"] == DBNull.Value) ? String.Empty : reader["COD_ALMACEN"].ToString();
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
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

        public List<ImportBE> ImportarDetInventario_Inventario(XmlDocument xml_import, string UserReg, int pass)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_ImportarDetalleInventario_Inventario_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;
                        comando.Parameters.Add("@Pass", SqlDbType.Int).Value = pass;
                        conexion.Open();
                        comando.ExecuteNonQuery();
                        //using (reader = comando.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        DetInventarioBE entity = new DetInventarioBE();
                        //        ImportBE Importobj = new ImportBE();
                        //        entity.Cod_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
                        //        entity.Cod_Ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                        //        entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                        //        entity.Lote_Prod = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                        //        entity.Serie_Prod = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();
                        //        entity.Stock_Inicial = (reader["STOCK"] == DBNull.Value) ? 0 : double.Parse(reader["STOCK"].ToString());
                        //        Importobj.Objeto = entity;
                        //        Importobj.Flg_pass = (reader["pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["pass"].ToString());
                        //        Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
                        //        Lista_result.Add(Importobj);
                        //    }
                        //}
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

        public Response Select_DET_INV_IMPORT(string start, string length, string order, string search)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("WEB_SLCT_DET_INV_IMPORT_2024", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        //comando.Parameters.Add("@P_XML_IMPORT", SqlDbType.Xml, 999999999).Value = xml_import.InnerXml.ToString();
                        //comando.Parameters.Add("@UserReg", SqlDbType.VarChar, 50).Value = UserReg;
                        //comando.Parameters.Add("@Pass", SqlDbType.Int).Value = pass;
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
                            }
                            reader.NextResult();
                            while (reader.Read())
                            {
                                DetInventarioBE entity = new DetInventarioBE();
                                ImportBE Importobj = new ImportBE();
                                entity.Cod_Inventario = (reader["COD_INVENTARIO"] == DBNull.Value) ? String.Empty : reader["COD_INVENTARIO"].ToString();
                                entity.Cod_Ubicacion = (reader["COD_UBICACION"] == DBNull.Value) ? String.Empty : reader["COD_UBICACION"].ToString();
                                entity.Cod_Producto = (reader["COD_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["COD_PRODUCTO"].ToString();
                                entity.Lote_Prod = (reader["LOTE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["LOTE_PRODUCTO"].ToString();
                                entity.Serie_Prod = (reader["SERIE_PRODUCTO"] == DBNull.Value) ? String.Empty : reader["SERIE_PRODUCTO"].ToString();
                                entity.Stock_Inicial = (reader["STOCK"] == DBNull.Value) ? 0 : double.Parse(reader["STOCK"].ToString());
                                Importobj.Objeto = entity;
                                Importobj.Flg_pass = (reader["pass"] == DBNull.Value) ? 0 : Int32.Parse(reader["pass"].ToString());
                                Importobj.Mensaje = (reader["Mensaje"] == DBNull.Value) ? String.Empty : reader["Mensaje"].ToString();
                                Lista_result.Add(Importobj);
                            }
                        }
                    }
                }
                response.Entity = Lista_result;
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

        public void Insert_ASF_DETALLE_INVENTARIO(string UserReg)
        {
            Response response = new Response();

            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            List<ImportBE> Lista_result = new List<ImportBE>();
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


        }


        #endregion
    }
}
