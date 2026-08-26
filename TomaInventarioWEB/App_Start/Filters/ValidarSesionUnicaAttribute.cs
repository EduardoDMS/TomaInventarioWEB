using BE;
using BL;
using System;
using System.Web;
using System.Web.Mvc;

namespace TomaInventarioWEB.Filters
{
    public class ValidarSesionUnicaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var session = httpContext.Session;
            var actionDescriptor = filterContext.ActionDescriptor;

            bool permiteAnonimo =
                actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (permiteAnonimo)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (session == null || session["UserID"] == null)
            {
                filterContext.Result = new RedirectResult("~/Seguridad/Login?motivo=sesion_requerida");
                return;
            }

            int idUsuario;

            try
            {
                idUsuario = Convert.ToInt32(session["UserID"]);
            }
            catch
            {
                session.Clear();
                session.Abandon();
                filterContext.Result = new RedirectResult("~/Seguridad/Login?motivo=sesion_invalida");
                return;
            }

            Guid? tokenLocal = null;

            if (session["TokenSesion"] != null)
            {
                if (session["TokenSesion"] is Guid)
                {
                    tokenLocal = (Guid)session["TokenSesion"];
                }
                else
                {
                    Guid tokenConvertido;

                    if (Guid.TryParse(session["TokenSesion"].ToString(), out tokenConvertido))
                    {
                        tokenLocal = tokenConvertido;
                    }
                }
            }
            //comprobamos el estado actual en la bd
            EstadoSesionBE estadoBD;

            try
            {
                estadoBD =
                    new SeguridadBL().ObtenerEstadoSesion(idUsuario);
            }
            catch
            {
                session.Clear();
                session.Abandon();
                filterContext.Result = new RedirectResult("~/Seguridad/Login?motivo=sesion_invalida");
                return;
            }
            //try
            //{
            //    estadoBD = new SeguridadBL().ObtenerEstadoSesion(idUsuario);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(
            //        $"Error al validar sesión. ID Usuario: {idUsuario}. " +
            //        $"Detalle: {ex.Message}", ex);
            //}

            // validar el estado

            bool sesionInvalida =
                estadoBD == null ||
                !estadoBD.FlgOnline ||
                tokenLocal == null ||
                estadoBD.TokenSesion == null ||
                estadoBD.TokenSesion.Value != tokenLocal.Value;

            if (sesionInvalida)
            {
                session.Clear();
                session.Abandon();
                filterContext.Result = new RedirectResult("~/Seguridad/Login?motivo=sesion_expirada");
                return;
            }
            // evitar el cache o algo asi 
            httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            httpContext.Response.Cache.SetNoStore();
            httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            base.OnActionExecuting(filterContext);
        }
    }
}