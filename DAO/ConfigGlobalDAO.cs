using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class ConfigGlobalDAO
    {
        private readonly string _cadena;
        
        public ConfigGlobalDAO(string cadena)
        {
            _cadena = cadena;
        }

        public bool GetMostrarLote()
        {
            using (var cn = new SqlConnection(_cadena))
            using (var cmd = new SqlCommand("SP_WEB_CONFIG_LOTE_GET", cn)) 
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                object r = cmd.ExecuteScalar();
                return r == null || r == DBNull.Value || Convert.ToBoolean(r);
            }
        }

        public async Task SetMostrarLoteAsync(bool valor)
        {
            using (var cn = new SqlConnection(_cadena))
            using (var cmd = new SqlCommand("SP_WEB_CONFIG_LOTE_SET", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@VALOR", SqlDbType.Bit).Value = valor;
                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
