using BE;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class SeguridadDAO
    {
        //public Response ValidarAcceso(string user, string pass)
        //{
        //    Response response = new Response();
        //    SqlConnection conn = null;
        //    SqlCommand cmd = null;
        //    SqlDataReader reader = null;
        //    //UsuarioLoginBE entity = new UsuarioLoginBE();
        //    try
        //    {
        //        using (conn = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (cmd = new SqlCommand("WEB_ValidarUsuario_2026", conn)) // se cambio el procedure wa
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Clear();
        //                cmd.Parameters.Add("@User", SqlDbType.VarChar, 20).Value = user;
        //                cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = pass;
        //                cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
        //                cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
        //                conn.Open();
        //                cmd.ExecuteNonQuery();
        //                response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
        //                response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
        //                //using (reader = cmd.ExecuteReader())
        //                //{
        //                //    while (reader.Read())
        //                //    {
        //                //        //entity.IdUsuario = (reader["ID_USUARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_USUARIO"].ToString());
        //                //        //entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
        //                //        //entity.Perfil = (reader["PERFIL_USUARIO"] == DBNull.Value) ? String.Empty : reader["PERFIL_USUARIO"].ToString();
        //                //        //entity.Activo = reader.GetBoolean(reader.GetOrdinal("FLG_ACTIVO"));
        //                //    }
        //                //}
        //            }
        //        }
        //        //response.Entity = entity;

        //    }
        //    catch (Exception e)
        //    {
        //        response.MENSAJE_ERROR = e.Message.ToString();
        //        response.HUBO_ERROR = true;
        //    }
        //    finally
        //    {
        //        if (conn != null) { conn.Close(); conn.Dispose(); }
        //        if (conn != null) conn.Dispose();
        //        if (reader != null) reader.Dispose();
        //    }
        //    return response;
        //}



        public Response ValidarAcceso(string user, string pass, bool forzarSesion)
        {
            Response response = new Response();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                using (conn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_ValidarUsuario_2026", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@User", SqlDbType.VarChar, 20).Value = user;
                        cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = pass;
                        cmd.Parameters.Add("@ForzarSesion", SqlDbType.Bit).Value = forzarSesion; 
                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CodigoResultado", SqlDbType.Int).Direction = ParameterDirection.Output; //canelita
                        cmd.Parameters.Add("@TokenSesion", SqlDbType.UniqueIdentifier).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
                        response.CodigoResultado = (cmd.Parameters["@CodigoResultado"].Value == DBNull.Value) ? 0 : (int)cmd.Parameters["@CodigoResultado"].Value;
                        response.TokenSesion = (cmd.Parameters["@TokenSesion"].Value == DBNull.Value) ? (Guid?)null : (Guid)cmd.Parameters["@TokenSesion"].Value;
                    }
                }
            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
                response.CodigoResultado = 1;
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return response;
        }


        public Response ObtenerUsuarioLog(string user, string pass)
        {
            Response response = new Response();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            UsuarioLoginBE entity = new UsuarioLoginBE();
            try
            {
                using (conn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_GetUsuarioLog_2024", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@User", SqlDbType.VarChar, 20).Value = user;
                        cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = pass;

                        conn.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.IdUsuario = (reader["ID_USUARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_USUARIO"].ToString());
                                entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
                                entity.Perfil = (reader["PERFIL_USUARIO"] == DBNull.Value) ? String.Empty : reader["PERFIL_USUARIO"].ToString();
                                //entity.Activo = reader.GetBoolean(reader.GetOrdinal("FLG_ACTIVO"));
                            }
                        }
                    }
                }
                response.Entity = entity;

            }
            catch (Exception e)
            {
                response.MENSAJE_ERROR = e.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
                if (conn != null) conn.Dispose();
                if (reader != null) reader.Dispose();
            }
            return response;
        }


        //deprecado
        //public Response LogoutUsuario(int idUsuario)
        //{
        //    Response response = new Response();
        //    SqlConnection cn = null;
        //    SqlCommand cmd = null;
        //    //SqlDataReader reader = null;
        //    try
        //    {
        //        using (cn = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (cmd = new SqlCommand("WEB_LogoutUsuario_2026", cn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@IdUsuario",SqlDbType.Int).Value = idUsuario;
        //                cn.Open();
        //                cmd.ExecuteNonQuery();
        //                //  cmd.CommandTimeout = 120;
        //            }
        //        }
        //    }
        //    catch (Exception e) 
        //    {
        //        response.HUBO_ERROR = true;
        //        response.MENSAJE_ERROR=e.Message.ToString();
        //    }
        //    return response;
        //}

        //public Guid? ObtenerTokenSesionActivo(int idUsuario)
        //{
        //    Guid? token = null;
        //    SqlConnection cn = null;
        //    SqlCommand cmd = null;

        //    try
        //    {
        //        using (cn = new SqlConnection(Connection.AppStringConection()))
        //        {
        //            using (cmd = new SqlCommand("WEB_obtenerTokenSesionActivo_2026", cn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
        //                cn.Open();
        //                object result = cmd.ExecuteScalar();
        //                if (result != null && result != DBNull.Value)
        //                {
        //                    token = (Guid)result;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        token = null;
        //        e.Message.ToString();
        //    }
        //    finally
        //    {
        //        if (cn != null) { cn.Close(); cn.Dispose(); }
        //    }
        //    return token;
        //}


        public EstadoSesionBE ObtenerEstadoSesion(int idUsuario)
        {
            EstadoSesionBE estado = null;

            using (SqlConnection cn = new SqlConnection(Connection.AppStringConection()))
            using (SqlCommand cmd = new SqlCommand("WEB_obtenerTokenSesionActivo_2026", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        estado = new EstadoSesionBE
                        {
                            FlgOnline = dr["FLG_ONLINE"] != DBNull.Value && Convert.ToInt32(dr["FLG_ONLINE"]) == 1,
                            TokenSesion = dr["TOKEN_SESION"] == DBNull.Value ? (Guid?)null : (Guid)dr["TOKEN_SESION"]
                        };
                    }
                }
            }

            return estado;
        }

        public void CerrarSesion(int idUsuario)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                using ( cn = new SqlConnection(Connection.AppStringConection()))
                {
                    using ( cmd = new SqlCommand("WEB_CerrarSesion_2026", cn))
                    {
                        cmd.CommandType= CommandType.StoredProcedure;
                        cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value=idUsuario;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e) 
            { 
                e.Message.ToString();
            }
            finally
            {
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }
    }
}
