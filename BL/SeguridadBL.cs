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
        public Response ValidarAcceso(string user, string pass,bool forzarSesion)
        {
            return new SeguridadDAO().ValidarAcceso(user, pass, forzarSesion);
        }

        public Response ObtenerUsuarioLog(string user, string pass)
        {
            return new SeguridadDAO().ObtenerUsuarioLog(user, pass);
        }

        //deprecado
        //public Response LogoutUsuario(int idUsuario)
        //{
        //    return new SeguridadDAO().LogoutUsuario(idUsuario);
        //}

        //public Guid? ObtenerTokenSesionActivo(int idUsuario)
        //{
        //    return new SeguridadDAO().ObtenerTokenSesionActivo(idUsuario);
        //}
        public EstadoSesionBE ObtenerEstadoSesion(int idUsuario)
        {
            return new SeguridadDAO().ObtenerEstadoSesion(idUsuario);
        }

        public void CerrarSesion(int idUsuario)
        {//que void bro 
            new SeguridadDAO().CerrarSesion(idUsuario);
        }
    }
}
