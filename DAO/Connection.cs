using System;
using System.Configuration;

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
                //String decryptedStr = Seguridad.DecryptAes(DecryptConnection);
                //return decryptedStr;
                return DecryptConnection;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
