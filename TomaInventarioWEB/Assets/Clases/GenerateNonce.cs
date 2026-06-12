using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTIL;

public class GenerateNonceAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        string nonce = Seguridad.GenerateNonce();
        filterContext.HttpContext.Items["ScriptNonce"] = nonce;
        base.OnActionExecuting(filterContext);
    }
}