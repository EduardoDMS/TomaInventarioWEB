using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public static class ConfigGlobalBL
    {
        private static readonly ConfigGlobalDAO _dao = new ConfigGlobalDAO(Connection.AppStringConection());
        public static bool MostrarLote => _dao.GetMostrarLote();
        public static Task SetMostrarLoteAsync(bool valor) => _dao.SetMostrarLoteAsync(valor);
    }
}
