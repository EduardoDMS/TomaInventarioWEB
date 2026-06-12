using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTIL;

namespace DAO
{
    public class Connection
    {
        public static String AppStringConection()
        {
            //return ConfigurationManager.ConnectionStrings["cnxAlmacen"].ConnectionString;
            String DecryptConnection = ConfigurationManager.ConnectionStrings["cnxAlmacen"].ConnectionString;
            try
            {
                String decryptedStr = Seguridad.DecryptAes(DecryptConnection);
                return decryptedStr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
