using System.Web.Mvc;
using System.Web.Routing;

public class CheckSession : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Session["UserID"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new
                {
                    controller = "Seguridad",
                    action = "Login"
                }));

            return;
        }

        base.OnActionExecuting(filterContext);
    }
}
