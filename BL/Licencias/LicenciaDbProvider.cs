using System;
using System.Data.SqlClient;
using DAO; 

namespace TomaInventario.BL.Licencias
{
    public class LicenciaDbProvider : ILicenciaProvider
    {
        private readonly string _connectionString;

        public LicenciaDbProvider()
        {
            _connectionString = Connection.AppStringConection();// a
        }

        public LicenciaConfig Obtener()
        {
            var config = new LicenciaConfig();

            using (var conn = new SqlConnection(_connectionString))
            {// consulta con withNolock (hace la consulta ultra rapida sin adquieir bloqueos) usar con precaucion, ya que puede traer datos inconsistentes si se esta modificando la tabla al mismo tiempo 
                string query = @"SELECT TOP 1
                    E.COD_EMPRESA,
                    E.DSC_EMPRESA,
                    E.FLG_ACTIVO,
                    E.RUC_EMPRESA,
                    E.DIR_EMPRESA,
                    E.TLF_EMPRESA,
                    E.RAZONSOCIAL_EMPRESA,
                    L.AlmacenesMax,
                    L.UbicacionesMax,
                    L.ProductosMax,
                    L.UsuariosAdmiMax,
                    L.UsuariosOpeMax,
                    L.InventariosPreparadosMax
                    FROM ASF_CONFIG_LICENCIA L WITH(NOLOCK)
                    INNER JOIN ASF_EMPRESA E WITH(NOLOCK) ON E.ID_EMPRESA = L.IdEmpresa
                    WHERE L.Estado = 1";

                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            config.COD_EMPRESA = reader["COD_EMPRESA"].ToString();
                            config.DESC_EMPRESA = reader["DSC_EMPRESA"].ToString();
                            config.RUC_EMPRESA = reader["RUC_EMPRESA"].ToString();
                            config.DIR_EMPRESA = reader["DIR_EMPRESA"].ToString();
                            config.TLF_EMPRESA = reader["TLF_EMPRESA"].ToString();
                            config.RAZONSOCIAL_EMPRESA = reader["RAZONSOCIAL_EMPRESA"].ToString();
                            config.AlmacenesMax = Convert.ToInt32(reader["AlmacenesMax"]);
                            config.UbicacionesMax = Convert.ToInt32(reader["UbicacionesMax"]);
                            config.ProductosMax = Convert.ToInt32(reader["ProductosMax"]);
                            config.UsuariosAdmiMax = Convert.ToInt32(reader["UsuariosAdmiMax"]);
                            config.UsuariosOpeMax = Convert.ToInt32(reader["UsuariosOpeMax"]);
                            config.InventariosPreparadosMax = Convert.ToInt32(reader["InventariosPreparadosMax"]);
                        }
                        else
                        {
                            // Fallback o contingencia en caso de que borren la fila por error
                            throw new Exception("No se encontró ninguna configuración de licencia activa en la base de datos.");
                        }
                    }
                }
            }

            return config;
        }
    }
}