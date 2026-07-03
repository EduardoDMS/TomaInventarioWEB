using DAO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Licencias
{
    public class LicenciaBL
    { // Recordar que al terminar el using se cierra la conexion automaticamente, por lo que no es necesario cerrarla manualmente
        public void InicializarLicencia(LicenciaConfig licencia)
        {
            using (SqlConnection cn = new SqlConnection(Connection.AppStringConection()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_INICIALIZAR_LICENCIA_DESDE_JSON", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Nuevos Campos Comerciales de Empresa
                    cmd.Parameters.AddWithValue("@CodEmpresa", licencia.COD_EMPRESA);
                    cmd.Parameters.AddWithValue("@DescEmpresa", licencia.DESC_EMPRESA);
                    cmd.Parameters.AddWithValue("@FlgActivo", licencia.FLG_ACTIVO ?? "1");
                    cmd.Parameters.AddWithValue("@RucEmpresa", licencia.RUC_EMPRESA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DirEmpresa", licencia.DIR_EMPRESA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TlfEmpresa", licencia.TLF_EMPRESA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RazonSocialEmpresa", licencia.RAZONSOCIAL_EMPRESA);

                    // Límites de Licencia
                    cmd.Parameters.AddWithValue("@AlmacenesMax", licencia.AlmacenesMax);
                    cmd.Parameters.AddWithValue("@UbicacionesMax", licencia.UbicacionesMax);
                    cmd.Parameters.AddWithValue("@ProductosMax", licencia.ProductosMax);
                    cmd.Parameters.AddWithValue("@UsuariosAdmiMax", licencia.UsuariosAdmiMax);
                    cmd.Parameters.AddWithValue("@UsuariosOpeMax", licencia.UsuariosOpeMax);
                    cmd.Parameters.AddWithValue("@InventariosPreparadosMax", licencia.InventariosPreparadosMax);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
