using System.Web;
using System.Web.Mvc;
using TomaInventarioWEB.Filters;

namespace TomaInventarioWEB
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidarSesionUnicaAttribute());
        }
    }
}
