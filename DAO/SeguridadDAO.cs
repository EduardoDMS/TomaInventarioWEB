using BE;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DAO
{
    public class SeguridadDAO
    {
        public Response ValidarAcceso(string user, string pass)
        {
            Response response = new Response();
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            //UsuarioLoginBE entity = new UsuarioLoginBE();
            try
            {
                using (conn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("WEB_ValidarUsuario_2026", conn)) // se cambio el procedure wa
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@User", SqlDbType.VarChar, 20).Value = user;
                        cmd.Parameters.Add("@Pass", SqlDbType.VarChar, 50).Value = pass;
                        cmd.Parameters.Add("@Hubo_error", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@msg", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        response.MENSAJE_ERROR = (string)cmd.Parameters["@msg"].Value ?? "";
                        response.HUBO_ERROR = (bool)cmd.Parameters["@Hubo_error"].Value;
                        //using (reader = cmd.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        //entity.IdUsuario = (reader["ID_USUARIO"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_USUARIO"].ToString());
                        //        //entity.Usuario = (reader["COD_USUARIO"] == DBNull.Value) ? String.Empty : reader["COD_USUARIO"].ToString();
                        //        //entity.Perfil = (reader["PERFIL_USUARIO"] == DBNull.Value) ? String.Empty : reader["PERFIL_USUARIO"].ToString();
                        //        //entity.Activo = reader.GetBoolean(reader.GetOrdinal("FLG_ACTIVO"));
                        //    }
                        //}
                    }
                }
                //response.Entity = entity;

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
    }


}
