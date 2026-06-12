using BE;
using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTIL;

namespace TomaInventarioWEB.Controllers
{
    public class SeguridadController : Controller
    {
        // GET: Seguridad
        [GenerateNonce]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult ValidarAcceso(string user, string pass)
        {
            var responseAcceso = new SeguridadBL().ValidarAcceso(user, pass);
            var objRspt = new Object[] { responseAcceso.HUBO_ERROR,responseAcceso.MENSAJE_ERROR,""};

            if (responseAcceso.HUBO_ERROR) { return Json(objRspt); }
            else {
                UsuarioLoginBE objUserLog = new UsuarioLoginBE();
                objUserLog = (UsuarioLoginBE) new SeguridadBL().ObtenerUsuarioLog(user, pass).Entity;
                Session["UserID"] = objUserLog.IdUsuario;
                Session["UserName"] = objUserLog.Usuario;
                Session["UserPerfil"] = objUserLog.Perfil;
                responseAcceso.MENSAJE_ERROR = "Ingreso Exitoso";
                objRspt[2] = "/Home/Index";
                return Json(objRspt);
            }
        }

        public ActionResult logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Seguridad");
        }

        public JsonResult afkSession() {
            string idstring=(Session["UserID"] == null )? String.Empty: Session["UserID"].ToString(); 
            return Json(idstring);
        }

    }
}
