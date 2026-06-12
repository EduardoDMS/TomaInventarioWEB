using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ConfigDAO
    {
        public Response GetCorreo()
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            CorreoBE entity = new CorreoBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_lISTAR_ConfigCorreo", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.SERVIDOR = (reader["SERVIDOR"] == DBNull.Value) ? String.Empty : reader["SERVIDOR"].ToString();
                                entity.PUERTO = (reader["PUERTO"] == DBNull.Value) ? String.Empty : reader["PUERTO"].ToString();
                                entity.REMITENTE = (reader["REMITENTE"] == DBNull.Value) ? String.Empty : reader["REMITENTE"].ToString();
                                entity.PRIORIDAD = (reader["PRIORIDAD"] == DBNull.Value) ? String.Empty : reader["PRIORIDAD"].ToString();
                                entity.FLG_HABILITAR = (reader["FLG_HABILITAR"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_HABILITAR"].ToString());
                                entity.FLG_AUTENTICACION = (reader["FLG_AUTENTICACION"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_AUTENTICACION"].ToString());
                                entity.USUARIO = (reader["USUARIO"] == DBNull.Value) ? String.Empty : reader["USUARIO"].ToString();
                                entity.CONTRASEÑA = (reader["CONTRASEÑA"] == DBNull.Value) ? String.Empty : reader["CONTRASEÑA"].ToString();
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
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (reader != null) reader.Dispose();
            }

            return response;

        }

        public Response GetEmpresa()
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            EmpresaBE entity = new EmpresaBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_EMPRESA_GETALL", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();

                        conexion.Open();

                        using (reader = comando.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entity.ID_EMPRESA = (reader["ID_EMPRESA"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_EMPRESA"].ToString());
                                entity.COD_EMPRESA = (reader["COD_EMPRESA"] == DBNull.Value) ? String.Empty : reader["COD_EMPRESA"].ToString();
                                entity.DSC_EMPRESA = (reader["DSC_EMPRESA"] == DBNull.Value) ? String.Empty : reader["DSC_EMPRESA"].ToString();
                                entity.FLG_ACTIVO = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                                entity.RUC_EMPRESA = (reader["RUC_EMPRESA"] == DBNull.Value) ? String.Empty : reader["RUC_EMPRESA"].ToString();
                                entity.DIR_EMPRESA = (reader["DIR_EMPRESA"] == DBNull.Value) ? String.Empty : reader["DIR_EMPRESA"].ToString();
                                entity.TLF_EMPRESA = (reader["TLF_EMPRESA"] == DBNull.Value) ? String.Empty : reader["TLF_EMPRESA"].ToString();
                                entity.RAZONSOCIAL_EMPRESA = (reader["RAZONSOCIAL_EMPRESA"] == DBNull.Value) ? String.Empty : reader["RAZONSOCIAL_EMPRESA"].ToString();
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
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (reader != null) reader.Dispose();
            }

            return response;

        }

        public Response SaveEmpresa(EmpresaBE empresa)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            EmpresaBE entity = new EmpresaBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_EMPRESA_ACTUALIZAR", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.Add("@pnIdEmpresa", SqlDbType.Int).Value = 1;
                        comando.Parameters.Add("@pvDscEmpresa", SqlDbType.VarChar,200).Value = empresa.DSC_EMPRESA;
                        comando.Parameters.Add("@pvRucEmpresa", SqlDbType.VarChar,20).Value = empresa.RUC_EMPRESA;
                        comando.Parameters.Add("@pvDirEmpresa", SqlDbType.VarChar,100).Value = empresa.DIR_EMPRESA;
                        comando.Parameters.Add("@pvTelEmpresa", SqlDbType.VarChar,20).Value = empresa.TLF_EMPRESA;
                        comando.Parameters.Add("@pvRazSocEmpresa", SqlDbType.VarChar, 100).Value = empresa.RAZONSOCIAL_EMPRESA;

                        conexion.Open();
                        comando.ExecuteReader();

                        //using (reader = comando.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        entity.ID_EMPRESA = (reader["ID_EMPRESA"] == DBNull.Value) ? 0 : Int32.Parse(reader["ID_EMPRESA"].ToString());
                        //        entity.COD_EMPRESA = (reader["COD_EMPRESA"] == DBNull.Value) ? String.Empty : reader["COD_EMPRESA"].ToString();
                        //        entity.DSC_EMPRESA = (reader["DSC_EMPRESA"] == DBNull.Value) ? String.Empty : reader["DSC_EMPRESA"].ToString();
                        //        entity.FLG_ACTIVO = (reader["FLG_ACTIVO"] == DBNull.Value) ? 0 : Int32.Parse(reader["FLG_ACTIVO"].ToString());
                        //        entity.RUC_EMPRESA = (reader["RUC_EMPRESA"] == DBNull.Value) ? String.Empty : reader["RUC_EMPRESA"].ToString();
                        //        entity.DIR_EMPRESA = (reader["DIR_EMPRESA"] == DBNull.Value) ? String.Empty : reader["DIR_EMPRESA"].ToString();
                        //        entity.TLF_EMPRESA = (reader["TLF_EMPRESA"] == DBNull.Value) ? String.Empty : reader["TLF_EMPRESA"].ToString();
                        //        entity.RAZONSOCIAL_EMPRESA = (reader["RAZONSOCIAL_EMPRESA"] == DBNull.Value) ? String.Empty : reader["RAZONSOCIAL_EMPRESA"].ToString();
                        //    }
                        //}
                    }
                }
                //response.Entity = entity;
                //response.Entity = entity;

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

        public Response SaveCorreo(CorreoBE correo)
        {
            Response response = new Response();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;
            EmpresaBE entity = new EmpresaBE();
            try
            {
                using (conexion = new SqlConnection(Connection.AppStringConection()))
                {
                    using (comando = new SqlCommand("ASF_SP_ACTUALIZAR_CONFIGURACION_CORREO", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        
                        comando.Parameters.Add("@servidor", SqlDbType.VarChar, 20).Value = correo.SERVIDOR;
                        comando.Parameters.Add("@puerto", SqlDbType.VarChar, 15).Value = correo.PUERTO;
                        comando.Parameters.Add("@remitente", SqlDbType.VarChar, 40).Value = correo.REMITENTE;
                        comando.Parameters.Add("@prioridad", SqlDbType.VarChar, 20).Value = correo.PRIORIDAD;
                        comando.Parameters.Add("@flg_habilitar", SqlDbType.Int).Value = correo.FLG_HABILITAR;
                        comando.Parameters.Add("@flg_autenticacion", SqlDbType.Int).Value = correo.FLG_AUTENTICACION;
                        comando.Parameters.Add("@usuario", SqlDbType.VarChar, 25).Value = correo.USUARIO;
                        comando.Parameters.Add("@contraseña", SqlDbType.VarChar, 25).Value = correo.CONTRASEÑA;

                        conexion.Open();
                        comando.ExecuteReader();

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
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (reader != null) reader.Dispose();
            }

            return response;

        }


    }
}
