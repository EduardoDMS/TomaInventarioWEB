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
        public int DuracionHoras { get; set; }
        public string Estado { get; set; }
        public int ConteosRealizados { get; set; }
    }

    public class ProductosKpiDao
    {
        public int ProductosInventariados { get; set; }
        public int ProductosConDiferencia { get; set; }
        public int ProductosFaltantes { get; set; }
        public int ProductosSobrantes { get; set; }
    }

    public class UbicacionesResultDao
    {
        //public int UbicacionesConDiferencias { get; set; }
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
        public List<UsuarioParticipacionDto> Usuarios { get; set; } = new List<UsuarioParticipacionDto>();
        // = new();
    }

    public class ReporteEjecutivoDAO
    {

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

                                info.DuracionHoras = dr["DuracionHoras"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DuracionHoras"]);
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
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_Ubicaciones_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;

                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {
                            UbicacionesResultDao resultado = new UbicacionesResultDao();

                                while (reader.Read())
                                {
                                    resultado.Top10.Add(new UbicacionDiferenciaDto
                                    {
                                        Codigo = reader["COD_UBICACION"].ToString(),
                                        StockInicial = Convert.ToDecimal(reader["STOCK_INICIAL"]),
                                        StockFinal = Convert.ToDecimal(reader["STOCK_FINAL"]),
                                        Diferencia = Convert.ToDecimal(reader["DIFERENCIA"])
                                    });
                                }
                                //      response.Entity = resultado;
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
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_KPI_2026", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;

                        con.Open();

                        using (reader = cmd.ExecuteReader())
                        {

                            ProductosKpiDao resultado = new ProductosKpiDao();
                            if (reader.Read())
                            {
                                

                                resultado.ProductosInventariados = Convert.ToInt32(reader["PRODUCTOS_INVENTARIADOS"]);

                                resultado.ProductosConDiferencia = Convert.ToInt32(reader["PRODUCTOS_CON_DIFERENCIA"]);

                                resultado.ProductosFaltantes = Convert.ToInt32(reader["PRODUCTOS_FALTANTES"]);

                                resultado.ProductosSobrantes = Convert.ToInt32(reader["PRODUCTOS_SOBRANTES"]);
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
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) { reader.Dispose(); }
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
                                resultado.SumaSobrantes = dr["SumaSobrantes"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SumaSobrantes"]);
                                resultado.SumaFaltantes = dr["SumaFaltantes"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SumaFaltantes"]);
                                resultado.DiferenciaNeta = dr["DiferenciaNeta"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DiferenciaNeta"]);
                            }

                            if (dr.NextResult())
                            {
                                while (dr.Read())
                                {
                                    resultado.Top10.Add(new ProductoDiferenciaDto
                                    {
                                        CodProducto = dr["COD_PRODUCTO"].ToString(),
                                        DscProducto = dr["DSC_PRODUCTO"].ToString(),
                                        Diferencia = Convert.ToDecimal( dr["DIFERENCIA"])
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
                                    resultado.Usuarios.Add(new UsuarioParticipacionDto
                                    {
                                        CodUsuario = reader["COD_USUARIO"].ToString(),
                                        NombreCompleto = reader["NOMBRE_COMPLETO"].ToString(),
                                        ProductosLecturados = Convert.ToInt32(reader["PRODUCTOS_LECTURADOS"])
                                        //CantLecturas = Convert.ToInt32(reader["CANT_LECTURAS"])
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

        public Response ObtenerFueraUbicacion(string codInventario)
        {
            Response response = new Response();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                using (cn = new SqlConnection(Connection.AppStringConection()))
                {
                    using (cmd = new SqlCommand("SP_WEB_ReporteEjecutivo_FueraUbicacion_2026", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@COD_INVENTARIO", SqlDbType.VarChar, 20).Value = codInventario;

                        cn.Open();

                        using (dr = cmd.ExecuteReader())
                        {
                            FueraUbicacionResultDao resultado = new FueraUbicacionResultDao();

                            while (dr.Read())
                            {
                                resultado.Productos.Add(
                                    new ProductoFueraUbicacionDto
                                    {
                                        CodProducto = dr["COD_PRODUCTO"].ToString(),
                                        DscProducto = dr["DSC_PRODUCTO"].ToString(),
                                        UbicacionInicial = dr["UBICACION_INICIAL"] == DBNull.Value ? null : dr["UBICACION_INICIAL"].ToString(),
                                        UbicacionContada = dr["UBICACION_CONTADA"] == DBNull.Value ? null : dr["UBICACION_CONTADA"].ToString()
                                    });
                            }
                            resultado.Total = resultado.Productos.Count;

                            resultado.Productos =
                                resultado.Productos
                                    .Take(10)
                                    .ToList();

                            response.Entity = resultado;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = ex.Message;
            }
            return response;
        }
    
    }

}
