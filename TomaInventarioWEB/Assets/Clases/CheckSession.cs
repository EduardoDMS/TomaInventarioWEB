using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class CheckSession : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session["UserID"] == null) 
        {
            //filterContext.Result = new RedirectResult("/Seguridad/Login"); 
            filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.ApplicationPath + "/Seguridad/Login");

        }

        base.OnActionExecuting(filterContext);
    }
}
