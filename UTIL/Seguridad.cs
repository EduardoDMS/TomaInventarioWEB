using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UTIL
{
    public class Seguridad
    {
        public static string EncryptAes(string value)
        {
            Aes256.Aes256 aes = new Aes256.Aes256(ConfigurationManager.AppSettings["KeyAes"].ToString(), ConfigurationManager.AppSettings["IvAes"].ToString());
            aes.EncryptString_Aes(value, out string msj);
            return msj;
        }
        public static string DecryptAes(string value)
        {
            Aes256.Aes256 aes = new Aes256.Aes256(ConfigurationManager.AppSettings["KeyAes"].ToString(), ConfigurationManager.AppSettings["IvAes"].ToString());
            aes.DecryptString_Aes(value, out string msj);
            return msj;
        }

        public static string GenerateNonce()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var nonceBytes = new byte[32];
                rng.GetBytes(nonceBytes);
                return Convert.ToBase64String(nonceBytes);
            }
        }
    }

}
