using BE;
using DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class SeguridadBL
    {
        public Response ValidarAcceso(string user, string pass)
        {
            return new SeguridadDAO().ValidarAcceso(user, pass);
        }

        public Response ObtenerUsuarioLog(string user, string pass)
        {
            return new SeguridadDAO().ObtenerUsuarioLog(user, pass);
        }
    }
}
