using BE;
using BL;
using System;
using System.Web.Mvc;

namespace TomaInventarioWEB.Controllers
{
    public class SeguridadController : Controller
    {
        // GET: Seguridad
        [GenerateNonce]
        [AllowAnonymous]
        public ActionResult Login()
        {
            //var provider = new LicenciaJsonProvider();
            //LicenciaConfig licencia = provider.Obtener();
            //Console.Write(licencia);
            return View();
        }

        //public ActionResult ValidarAcceso(string user, string pass,bool forzarSesion = false)
        //{
        //    var responseAcceso = new SeguridadBL().ValidarAcceso(user, pass, forzarSesion);
        //    var objRspt = new Object[] { responseAcceso.HUBO_ERROR, responseAcceso.MENSAJE_ERROR, "" };

        //    if (responseAcceso.HUBO_ERROR) { return Json(objRspt); }
        //    else
        //    {
        //        UsuarioLoginBE objUserLog = new UsuarioLoginBE();
        //        objUserLog = (UsuarioLoginBE)new SeguridadBL().ObtenerUsuarioLog(user, pass).Entity;
        //        Session["UserID"] = objUserLog.IdUsuario;
        //        Session["UserName"] = objUserLog.Usuario;
        //        Session["UserPerfil"] = objUserLog.Perfil;
        //        responseAcceso.MENSAJE_ERROR = "Ingreso Exitoso";
        //        objRspt[2] = "/Home/Index";
        //        return Json(objRspt);
        //    }
        //}

        [AllowAnonymous]
        public ActionResult ValidarAcceso(string user, string pass , bool forzarSesion = false)
        {
            var responseAcceso = new SeguridadBL().ValidarAcceso(user, pass, forzarSesion);

            if (responseAcceso.HUBO_ERROR)
            {
                var objRspt = new Object[] { true, responseAcceso.MENSAJE_ERROR, "", responseAcceso.CodigoResultado };
                return Json(objRspt);
            }
            else
            {
                UsuarioLoginBE objUserLog = (UsuarioLoginBE)new SeguridadBL().ObtenerUsuarioLog(user, pass).Entity;
                Session["UserID"] = objUserLog.IdUsuario;
                Session["UserName"] = objUserLog.Usuario;
                Session["UserPerfil"] = objUserLog.Perfil;
                Session["TokenSesion"] = responseAcceso.TokenSesion; // NUEVO: guarda cuál es "mi" token

                var objRspt = new Object[] { false, "Ingreso Exitoso", "/Home/Index", 0 };
                return Json(objRspt);
            }
        }


        public ActionResult logout()
        {
            if (Session["UserID"] != null)
            {
                int idUsuario = Convert.ToInt32(Session["UserID"]);
                new SeguridadBL().CerrarSesion(idUsuario);
            }
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Seguridad");
        }

        public JsonResult afkSession()
        {
            string idstring = (Session["UserID"] == null) ? String.Empty : Session["UserID"].ToString();
            return Json(idstring);
        }

    }
}
