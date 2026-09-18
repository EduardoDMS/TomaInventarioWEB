using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BE;

namespace DAO.ReporteResumenEjecutivo
{
    // DTOs crudos de cada SP, sin logica de negocio
    public class InfoInventarioDao
    {
        public string CodAlmacen { get; set; }
        public string DscAlmacen { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierreFinal { get; set; }
        public int DuracionDias { get; set; }
        public string Estado { get; set; }
        public int ConteosRealizados { get; set; }
    }

    public class ProductosKpiDao
    {
        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
    }

    public class UbicacionesResultDao
    {
        public int UbicacionesConDiferencias { get; set; }
        public List<UbicacionDiferenciaDto> Top10 { get; set; } = new List<UbicacionDiferenciaDto>();
        // = new();
    }

    public class DiferenciasResultDao
    {
        public decimal SumaSobrantes { get; set; }
        public decimal SumaFaltantes { get; set; }
        public decimal DiferenciaNeta { get; set; }
        public List<ProductoDiferenciaDto> Top10 { get; set; } = new List<ProductoDiferenciaDto>();
        // = new();
    }

    public class UsuariosResultDao
    {
        public int UsuariosParticipantes { get; set; }
        public List<UsuarioParticipacionDto> Top10 { get; set; } = new List<UsuarioParticipacionDto>();
        // = new();
    }

    public class ReporteEjecutivoDAO
    {
        ////private readonly string _connectionString;

        //public ReporteEjecutivoDao(string connectionString)
        //{
        //    //_connectionString = connectionString;
        //}

        public Response ObtenerInfo(string codInventario)
        {
            Response response = new Response();

            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                using(cn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_Info_2026", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        cn.Open();

                        using (dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                InfoInventarioDao info = new InfoInventarioDao();

                                info.CodAlmacen = dr["COD_ALMACEN"].ToString();
                                info.DscAlmacen = dr["DSC_ALMACEN"].ToString();
                                info.FechaInicio = Convert.ToDateTime(dr["FechaInicio"]);

                                if (dr["FechaCierreFinal"] != DBNull.Value)
                                {
                                    info.FechaCierreFinal = Convert.ToDateTime(dr["FechaCierreFinal"]);
                                }
                                else
                                {
                                    info.FechaCierreFinal = null;
                                }

                                info.DuracionDias = dr["DuracionDias"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DuracionDias"]);
                                info.Estado = dr["Estado"].ToString();
                                info.ConteosRealizados = Convert.ToInt32(dr["ConteosRealizados"]);

                                response.Entity = info;
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                response.MENSAJE_ERROR = ex.Message.ToString();
                response.HUBO_ERROR = true;
            }
            finally
            {
                if(cn != null) { cn.Close(); cn.Dispose(); }
                if(dr != null) { dr.Dispose(); }
            }
            return response;
        }

        public Response ObtenerUbicacionesConDiferencia(string codInventario)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_reporteXubicaciones_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        cmd.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = "";
                        cmd.Parameters.Add("@P_ESTADO", SqlDbType.VarChar, 20).Value = DBNull.Value;
                        cmd.Parameters.Add("@P_SOLO_DIFERENCIAS", SqlDbType.Bit).Value = 1;
                        cmd.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = "ABS(DIFERENCIA) DESC";

                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            UbicacionesResultDao resultado = new UbicacionesResultDao();

                            // Primer resultado = KPI
                            // Segundo resultado = detalle de ubicaciones
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.UbicacionesConDiferencias++;

                                    if (resultado.Top10.Count < 10)
                                    {
                                        resultado.Top10.Add(new UbicacionDiferenciaDto
                                        {
                                            Codigo = reader["CODIGO"].ToString(),
                                            Ubicacion = reader["UBICACION"] == DBNull.Value ? null : reader["UBICACION"].ToString(),
                                            Diferencia = Convert.ToDecimal(reader["DIFERENCIA"])
                                        });
                                    }
                                }
                            }

                            response.Entity = resultado;
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
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }

                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return response;
        }




        public Response ObtenerProductosKpi(string codInventario)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_reporteXproducto_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;
                        cmd.Parameters.Add("@BUSQUEDA", SqlDbType.VarChar, 200).Value = "";
                        cmd.Parameters.Add("@P_OBSERVACION", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@P_IDSTART", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@P_ESTADO", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@P_ORDER", SqlDbType.VarChar, 50).Value = DBNull.Value;

                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            ProductosKpiDao resultado = new ProductosKpiDao();

                            if (reader.Read())
                            {
                                resultado.ProductosInventariados =
                                    Convert.ToInt32(reader["PRODUCTOS_INVENTARIADOS"]);

                                resultado.ProductosConDiferencia =
                                    Convert.ToInt32(reader["PRODUCTOS_CON_DIFERENCIA"]);
                            }

                            response.Entity = resultado;
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
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }

                if (reader != null)
                {
                    reader.Dispose();
                }
            }

            return response;
        }



        public Response ObtenerDiferencias(string codInventario)
        {
            Response response = new Response();

            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                using (cn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_Diferencias_2026", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;

                        cn.Open();

                        using (dr = cmd.ExecuteReader())
                        {
                            DiferenciasResultDao resultado = new DiferenciasResultDao();

                            if (dr.Read())
                            {
                                resultado.SumaSobrantes = Convert.ToDecimal(dr["SumaSobrantes"]);
                                resultado.SumaFaltantes = Convert.ToDecimal(dr["SumaFaltantes"]);
                                resultado.DiferenciaNeta = Convert.ToDecimal(dr["DiferenciaNeta"]);
                            }

                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    resultado.Top10.Add(new ProductoDiferenciaDto
                                    {
                                        CodProducto = dr["COD_PRODUCTO"].ToString(),
                                        DscProducto = dr["DSC_PRODUCTO"].ToString(),
                                        Diferencia = Convert.ToDecimal(dr["DIFERENCIA"])

                                    });
                                }
                            }
                            response.Entity = resultado;
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
                if (cn != null) { cn.Close(); cn.Dispose(); }
                if (dr != null) { dr.Close(); dr.Dispose(); }
            }
            return response;
        }


        public Response ObtenerUsuarios(string codInventario)
        {
            Response response = new Response();

            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                using (con = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_Usuarios_2026",con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;
                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value =codInventario;
                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            UsuariosResultDao resultado = new UsuariosResultDao();

                            if (reader.Read())
                            {
                                resultado.UsuariosParticipantes =Convert.ToInt32(reader["USUARIOS_PARTICIPANTES"]);
                            }

                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    resultado.Top10.Add(new UsuarioParticipacionDto
                                    {
                                        CodUsuario = reader["COD_USUARIO"].ToString(),
                                        NombreCompleto = reader["NOMBRE_COMPLETO"].ToString(),
                                        CantLecturas = Convert.ToInt32(reader["CANT_LECTURAS"])
                                    });
                                }
                            }
                            response.Entity = resultado;
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
                if (con != null) { con.Close();con.Dispose();}
                if (reader != null) { reader.Dispose(); }
            }
            return response;
        }
    }

}
