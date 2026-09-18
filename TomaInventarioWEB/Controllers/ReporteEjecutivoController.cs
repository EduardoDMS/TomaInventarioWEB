
using BE;
using BL;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class ReporteEjecutivoController : Controller
    {
        [GenerateNonce]
        public ActionResult EnvioAlmacen()
        {
            Session["NavIndex"] = "4";

            CombosBE objComboAlmacen = new CombosBE();

            List<CombosBE> listaAlmacenes = new List<CombosBE>();

            listaAlmacenes =
                (List<CombosBE>)new CombosBL()
                    .cbxAlmacenes()
                    .Entity;

            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }

        [HttpPost]
        public JsonResult FillCbxInventariosPorAlmacen(string idAlmacen)
        {
            //return Json(new
            //{
            //    success = true,
            //    recibido = idAlmacen
            //});
            try
            {
                CombosBE objCombo = new CombosBE();
                objCombo.vchValue = "-1";
                objCombo.vchdesc = "Seleccionar Inventario";

                List<CombosBE> listaInventariosPorAlmacen = new List<CombosBE>();
                listaInventariosPorAlmacen = (List<CombosBE>)new CombosBL().cbxInventariosPorAlmacen(idAlmacen).Entity;
                listaInventariosPorAlmacen.Insert(0, objCombo);

                return Json(new { success = true, data = listaInventariosPorAlmacen });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }



        //[GenerateNonce]
        //public ActionResult EnvioAlmacen()
        //{
        //    Session["NavIndex"] = "4";

        //    var response = new CombosBL().cbxAlmacenes();

        //    ViewBag.ListaAlmacenes =
        //        (System.Collections.Generic.List<CombosBE>)response.Entity;

        //    return View("Reporte_Ejecutivo");
        //}


        // GET: /ReporteEjecutivo/Pdf?codInventario=INV_310726_1758
        [HttpGet]
        public ActionResult Pdf(string codInventario)
        {
            if (string.IsNullOrWhiteSpace(codInventario))
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "Falta el código de inventario"
                );
            }

            ReporteEjecutivoViewModel vm;

            try
            {
                var bl = new ReporteEjecutivoService();

                vm = bl.ObtenerReporte(codInventario);
            }
            catch (InvalidOperationException ex)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.NotFound,
                    ex.Message
                );
            }

            var documento =
                (IDocument)new ReporteEjecutivoDocument(vm);

            byte[] pdfBytes = documento.GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                $"ReporteEjecutivo_{codInventario}.pdf"
            );
        }
    }
}
