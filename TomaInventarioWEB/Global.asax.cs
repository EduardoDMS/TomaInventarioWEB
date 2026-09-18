using BL.Licencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QuestPDF.Infrastructure;

namespace TomaInventarioWEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            QuestPDF.Settings.License =LicenseType.Evaluation;
            QuestPDF.Settings.UseSystemFonts = true; // para la fuente de letra csmr ERROR EN SERVIDOR IIS SI NO SE HACE CORRECTAMENTE 
            QuestPDF.Settings.ThrowOnMissingFontFamilies = false;
            new LicenciaInitializer().Inicializar();// agregada la licencia :v
        }
        //protected void Session_End(object sender, EventArgs e)
        //{
        //    if (Session["UserID"] == null)
        //    {
        //        Response.Redirect("~/Login.aspx");
        //    }
        //}
    }
}
