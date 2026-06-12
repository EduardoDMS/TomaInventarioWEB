using System;
using System.Web;
using System.Web.Optimization;

namespace TomaInventarioWEB
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {           
            bundles.Add(new StyleBundle("~/bundles/css-vendor")
            .Include(
                "~/Content/matdash/libs/bootstrap/dist/css/bootstrap.min.css",
                "~/Content/matdash/libs/jquery-ui/dist/themes/base/jquery-ui.min.css",
                "~/Content/matdash/fonts/tabler-icons/tabler-icons.min.css",
                "~/Content/matdash/libs/sweetalert2/dist/sweetalert2.min.css",
                "~/Content/matdash/libs/select2/dist/css/select2.min.css",
                "~/Assets/CSS/cdn/select2-bootstrap-5-theme@1.3.0-dist-select2-bootstrap-5-theme.min.css",
                "~/Content/matdash/libs/datatables.net-bs5/css/dataTables.bootstrap5.min.css",
                "~/Assets/CSS/cdn/responsive.dataTables.min.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/css-app")
            .Include(
                "~/Content/matdash/css/fonts.css",
                "~/Content/matdash/css/styles.css",
                "~/Assets/CSS/Estilos-generales_nuevo.css",
                "~/Assets/CSS/Login_nuevo.css",
                "~/Assets/CSS/Header_nuevo.css",
                "~/Assets/CSS/Sidebar_nuevo.css"              
            ));

            bundles.Add(new ScriptBundle("~/bundles/js-jquery")
            .Include(
                "~/Scripts/jquery-3.7.1.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/js-plugins")
            .Include(
                "~/Content/matdash/libs/bootstrap/dist/js/bootstrap.bundle.min.js", 
                "~/Content/matdash/libs/jquery-ui/dist/jquery-ui.min.js",
                "~/Content/matdash/libs/sweetalert2/dist/sweetalert2.min.js",
                "~/Content/matdash/libs/select2/dist/js/select2.min.js",
                "~/assets/js/cdn/1.13.8-js-datatables.min.js",
                "~/assets/js/cdn/1.13.8-datatables.responsive.min.js"
            ));
        }
    }
}
