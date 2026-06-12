using BE;
using BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class ConfiguracionController : Controller
    {
        // GET: Configuracion
        [GenerateNonce]
        public ActionResult ConfigGeneral(String Seg)
        {
            Session["NavIndex"] = "5";
            ViewBag.SubNavIndex = Seg; 

            return View();
        }

        [GenerateNonce]
        public ActionResult ConfigGeneral_Empresa()
        {
            return View();
        }

        [GenerateNonce]
        public ActionResult ConfigGeneral_Correo()
        {
            return View();
        }

        [GenerateNonce]
        public ActionResult ConfigGeneral_Personalizacion()
        {
            return View();
        }


        public ActionResult GuardarImagenBanner(string imagen)
        {
            if (!string.IsNullOrEmpty(imagen))
            {
                // Decodifica la imagen base64 y conviértela a bytes
                byte[] imagenBytes = Convert.FromBase64String(imagen);

                // Ruta de la carpeta en la que deseas guardar la imagen
                string carpetaDestino = Server.MapPath("~/Assets/IMG/");

                // Asegúrate de que la carpeta exista
                if (!Directory.Exists(carpetaDestino))
                {
                    Directory.CreateDirectory(carpetaDestino);
                }

                // Genera un nombre de archivo único para la imagen
                //string nombreArchivo = Guid.NewGuid().ToString() + ".png"; // Cambia la extensión según tu caso
                string nombreArchivo = "BannerActual.png"; // Cambia la extensión según tu caso

                // Ruta completa del archivo
                string rutaCompletaArchivo = Path.Combine(carpetaDestino, nombreArchivo);

                // Guarda la imagen en la carpeta
                System.IO.File.WriteAllBytes(rutaCompletaArchivo, imagenBytes);

                //Puedes devolver una respuesta JSON u otro indicador de éxito
                return Json(new { success = true, mensaje = "Imagen guardada exitosamente" });
            }

            return Json(new { success = false, mensaje = "Error al guardar la imagen" });
        }
        public ActionResult EliminarImagenBanner()
        {
            string rutaImagen = Server.MapPath("~/Assets/IMG/BannerActual.png");

            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        public ActionResult GuardarImagenExcel()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = "LogoExcel.png";
                        var path = Path.Combine(Server.MapPath("~/Assets/IMG/"), fileName);
                        file.SaveAs(path);
                        return new HttpStatusCodeResult(200); // OK
                    }
                }
                return new HttpStatusCodeResult(400); // Bad Request
            }
            catch (Exception ex)
            {
                // Manejar errores
                return new HttpStatusCodeResult(500); // Internal Server Error
            }
        }
        public ActionResult EliminarImagenExcel()
        {
            string rutaImagenExcel = Server.MapPath("~/Assets/IMG/LogoExcel.png");

            if (System.IO.File.Exists(rutaImagenExcel))
            {
                System.IO.File.Delete(rutaImagenExcel);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public ActionResult GuardarImagenFondo()
        {

            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        //var fileName = Path.GetFileName(file.FileName);
                        var fileName = "fondo_principal.png";
                        var path = Path.Combine(Server.MapPath("~/Assets/IMG/"), fileName);
                        file.SaveAs(path);
                        return new HttpStatusCodeResult(200); // OK
                    }
                }
                return new HttpStatusCodeResult(400); // Bad Request
            }
            catch (Exception ex)
            {
                // Manejar errores
                return new HttpStatusCodeResult(500); // Internal Server Error
            }
        }
        public ActionResult EliminarImagenFondo()
        {
            string rutaImagenFondo = Server.MapPath("~/Assets/IMG/fondo_principal.png");

            if (System.IO.File.Exists(rutaImagenFondo))
            {
                System.IO.File.Delete(rutaImagenFondo);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult GetCorreo()
        {
            var response = new ConfigBL().GetCorreo();
            return Json(response.Entity);
        }

        [HttpPost]
        public JsonResult GetEmpresa()
        {
            var response = new ConfigBL().GetEmpresa();
            return Json(response.Entity);
        }
        public JsonResult SaveEmpresa(EmpresaBE obj)
        {
            var response = new ConfigBL().SaveEmpresa(obj);
            return Json(response);
        }
        public JsonResult SaveCorreo(CorreoBE obj)
        {
            var response = new ConfigBL().SaveCorreo(obj);
            return Json(response);
        }
    }
}