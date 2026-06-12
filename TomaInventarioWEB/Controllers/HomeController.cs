using BE;
using BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]

    public class HomeController : Controller
    {
        [GenerateNonce]
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public JsonResult ObtenerDatosDashboard(int? idEmpresa = null, int? anio = null)
        {
            try
            {
                DashboardBL dashboardBL = new DashboardBL();
                Response response = dashboardBL.ObtenerDatosDashboard(idEmpresa, anio);

                if (response.HUBO_ERROR)
                {
                    return Json(new
                    {
                        success = false,
                        message = response.MENSAJE_ERROR
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "Datos obtenidos correctamente",
                    data = response.Entity
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al obtener datos del dashboard: " + ex.Message
                });
            }
        }


        [GenerateNonce]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [GenerateNonce]
        public ActionResult Ayuda()
        {
            return View();
        }
    }
}