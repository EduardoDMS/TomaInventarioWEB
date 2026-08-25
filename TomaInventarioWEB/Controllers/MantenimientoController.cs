using Antlr.Runtime.Misc;
using BE;
using BL;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class MantenimientoController : Controller
    {
        // GET: Mantenimiento

        #region Mantenimiento de ubicaciones
        [GenerateNonce]
        public ActionResult UbicacionesMant()
        {
            Session["NavIndex"] = "4";

            List<CombosBE> listAlmacenes = new List<CombosBE>();
            
            CombosBE objCombo = new CombosBE();
            objCombo.intValue = -1;
            objCombo.vchdesc = "Todos";
            listAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            ViewBag.ListaAlmacenesModal = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listAlmacenes.Insert(0, objCombo);

            ViewBag.ListaAlmacenes = listAlmacenes;

            return View();
        }


        [HttpPost]
        //public JsonResult ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen)
        //{
        //    MantenimientosBL response = new MantenimientosBL();
        //    List<UbicacionBE> List = response.ListarUbicaciones(activo, vchUbicacion, idAlmacen);
        //    var json = Json(new { data = List });
        //    return json;
        //}


        public JsonResult ListarUbicaciones(string activo, string vchUbicacion, int idAlmacen, string start, string length, int draw)
        {
            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            var response = new MantenimientosBL().ListarUbicaciones(activo, vchUbicacion, idAlmacen,start, length, order);
            List<UbicacionBE> lista = new List<UbicacionBE>();
            lista = (List<UbicacionBE>)response.Entity;
            //var pagedData = lista.Skip(0).Take(10).ToList();
            var totalData = response.count;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = lista });

            //var json = Json(new { data = List });
            //return json;
        }

        [HttpPost]
        public JsonResult CreateUbicacion(string codUbicacion, string dscUbicacion, List<string> ListidAlmacen)
        {
            var response = new MantenimientosBL().CreateUbicacion(codUbicacion, dscUbicacion, ListidAlmacen);
            return Json(response);
        }

        [HttpPost]
        public JsonResult GetUbicacion(string CodUbicacion)
        {
            var response = new MantenimientosBL().GetUbicacion(CodUbicacion);
            return Json(response);
        }
        [HttpPost]
        public JsonResult UpdateUbicacion(string codUbicacion, string DSCUbicacion, List<string> ListidAlmacen, bool activo)
        {
            var response = new MantenimientosBL().UpdateUbicacion(codUbicacion, DSCUbicacion, ListidAlmacen, activo);
            return Json(response);
        }
        #endregion

        #region Mantenimiento de almacenes
        [GenerateNonce]
        public ActionResult AlmacenesMant()
        {
            Session["NavIndex"] = "4";

            return View();
        }


        [HttpPost]
        public JsonResult ListarAlmacenes(string dscAlmacen, string activo)
        {
            MantenimientosBL response = new MantenimientosBL();
            List<AlmacenBE> List = response.ListarAlmacenes(dscAlmacen, activo);
            var json = Json(new { data = List });
            return json;
        }
        [HttpPost]
        public JsonResult CreateAlmacen(string codAlmacen, string descAlmacen)
        {
            var response = new MantenimientosBL().CreateAlmacen(codAlmacen, descAlmacen);
            return Json(response);
        }
        [HttpPost]
        public JsonResult GetAlmacen(int idAlmacen)
        {
            var response = new MantenimientosBL().GetAlmacen(idAlmacen);
            return Json(response);
        }
        [HttpPost]
        public JsonResult UpdateAlmacen(int idAlmacen, string codAlmacen, string dscAlmacen, bool activo)
        {
            var response = new MantenimientosBL().UpdateAlmacen(idAlmacen, codAlmacen, dscAlmacen, activo);
            return Json(response);
        }
        [HttpPost]
        public JsonResult CargarAlmacenesAPIExterna()
        {
            UTIL.APIExterna api = new UTIL.APIExterna();
            BL.MantenimientosBL BLresponse = new BL.MantenimientosBL();
            Response response = BLresponse.GetAPIAlmacen();
            string APIAlmacen;

            if (response.HUBO_ERROR == false){ APIAlmacen = response.MENSAJE_ERROR;}
            else {
                response.HUBO_ERROR = true;
                return Json(response);
            }
            
            var ApiResponse = api.ObtenerALMACENES_IQFARMA(APIAlmacen);
            if (ApiResponse == "")
            {
                response.MENSAJE_ERROR = "No se obtuvo respuesta de la API o la lista se encuetra vacia";
                return Json(response);
            }
            List<AlmacenAPI> ListAlmacen = new List<AlmacenAPI>();
            ListAlmacen = Almacen_Convert_XML_List(ApiResponse);

            
            //foreach (List<AlmacenAPI> listaBloque in SplitListIntoChunks(ListAlmacen, 100))
            //{
            //    BLresponse.CargarAlmacenesAPIExterna(listaBloque);
            //}

            
            response = BLresponse.CargarAlmacenesAPIExterna(ListAlmacen);
            return Json(response);
        }

        //private List<List<AlmacenAPI>> SplitListIntoChunks(List<AlmacenAPI> lista, int chunkSize)
        //{
        //    return lista.Select((x, i) => new { Index = i, Value = x })
        //        .GroupBy(x => x.Index / chunkSize)
        //        .Select(x => x.Select(v => v.Value).ToList())
        //        .ToList();
        //}


        public static List<AlmacenAPI> Almacen_Convert_XML_List(string xmlResponse)
        {
            try
            {
                // Cargar el XML y extraer el contenido JSON
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);
                string jsonContent = doc.InnerText; // Extrae el JSON dentro del nodo <string>

                // Deserializar el JSON completo
                dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);

                // Convertir la sección "Datos" en una lista de objetos CLISTA_DETALLE
                string datosJson = JsonConvert.SerializeObject(jsonData.Datos);
                List<AlmacenAPI> lista = JsonConvert.DeserializeObject<List<AlmacenAPI>>(datosJson);

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar la respuesta del API: " + ex.Message);
                return new List<AlmacenAPI>(); // Retorna lista vacía en caso de error
            }
        }


        #endregion

        #region Mantenimiento de usuarios
        [GenerateNonce]
        public ActionResult UsuariosMant()
            {
                Session["NavIndex"] = "4";

                List<CombosBE> listAlmacenes = new List<CombosBE>();
                listAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
                ViewBag.ListaAlmacenes = listAlmacenes;

                return View();
            }

            [HttpPost]
            public JsonResult ListarUsuarios(string perfil, string usuario)
            {
                MantenimientosBL response = new MantenimientosBL();
                List<UsuarioBE> UserList = response.ListarUsuarios(perfil, usuario);
                var json = Json(new { data = UserList });
                return json;
            }
            
            [HttpPost]
            public JsonResult CreateUsuario(string cod_usuario, string nombre, string apellido, string clave, string perfil, List<string> idAlmacen)
            {
                var response = new MantenimientosBL().CreateUsuario(cod_usuario,nombre, apellido, clave, perfil, idAlmacen);
                return Json(response);
            }
            
            [HttpPost]
            public JsonResult GetUsuario(int idUsuario)
            {
                var response = new MantenimientosBL().GetUsuario(idUsuario);
                return Json(response);
            }
            
            [HttpPost]
            public JsonResult UpdateUsuario(int idUsuario, string cod_usuario, string nombreUsuario, string apellidoUsuario, string clave, string perfil, List<string> idAlmacen, bool activo)
            {
                var response = new MantenimientosBL().UpdateUsuario(idUsuario, cod_usuario, nombreUsuario, apellidoUsuario, clave, perfil, idAlmacen, activo);
                return Json(response);
            }

        #endregion

        #region Mantenimiento de Productos
        [GenerateNonce]
        public ActionResult ProductosMant()
        {
            Session["NavIndex"] = "4";

            List<CombosBE> listUM = new List<CombosBE>();

            listUM = (List<CombosBE>)new CombosBL().cbxUM().Entity;
            ViewBag.ListaUM = listUM;

            List<CombosBE> listMon = new List<CombosBE>();

            listMon = (List<CombosBE>)new CombosBL().cbxMon().Entity;
            ViewBag.listMon = listMon;

            return View();
        }

        [HttpPost]
        public JsonResult ListarProductos(int activo, string vchProducto, string start, string length, int draw)
        {
            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            var response = new MantenimientosBL().ListarProductos(activo, vchProducto, start, length, order);
            List<ProductoBE> lista = new List<ProductoBE>();
            lista = (List<ProductoBE>)response.Entity;
            //var pagedData = lista.Skip(0).Take(10).ToList();
            var totalData = response.count;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = lista });

            //var json = Json(new { data = List });
            //return json;
        }

        [HttpPost]
        public JsonResult CreateProducto(string codProducto, string descProducto, int UM, decimal costo, int idMoneda)
        {
            var response = new MantenimientosBL().CreateProducto(codProducto, descProducto, UM, costo, idMoneda);
            return Json(response);
        }

        [HttpPost]
        public JsonResult GetProducto(int id)
        {
            var response = new MantenimientosBL().GetProducto(id);
            return Json(response);
        }
       
        [HttpPost]
        public JsonResult UpdateProducto(int id, string codProducto, string descProducto, bool activo,decimal costo, int idMoneda, int UM)
        {
            var response = new MantenimientosBL().UpdateProducto(id, codProducto, descProducto, Session["UserName"].ToString(), activo,costo, idMoneda, UM);
            return Json(response);
        }

        [HttpPost]
        public JsonResult CargarProductosAPIExterna()
        {
            UTIL.APIExterna api = new UTIL.APIExterna();
            BL.MantenimientosBL BLresponse = new BL.MantenimientosBL();
            Response response = BLresponse.GetAPIProductos();
            string APIAlmacen;

            if (response.HUBO_ERROR == false) { APIAlmacen = response.MENSAJE_ERROR; }
            else
            {
                response.HUBO_ERROR = true;
                return Json(response);
            }

            var ApiResponse = api.ObtenerProductos(APIAlmacen);
            if (ApiResponse == "")
            {
                response.MENSAJE_ERROR = "No se obtuvo respuesta de la API o la lista se encuetra vacia";
                return Json(response);
            }
            List<ProductoBEAPI> ListProductos = new List<ProductoBEAPI>();
            ListProductos = Producto_Convert_XML_List(ApiResponse);


            //foreach (List<AlmacenAPI> listaBloque in SplitListIntoChunks(ListAlmacen, 100))
            //{
            //    BLresponse.CargarAlmacenesAPIExterna(listaBloque);
            //}


            response = BLresponse.CargarProductosAPIExterna(ListProductos);
            return Json(response);
        }
        public static List<ProductoBEAPI> Producto_Convert_XML_List(string xmlResponse)
        {
            try
            {
                // Cargar el XML y extraer el contenido JSON
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);
                string jsonContent = doc.InnerText; // Extrae el JSON dentro del nodo <string>

                // Deserializar el JSON completo
                dynamic jsonData = JsonConvert.DeserializeObject(jsonContent);

                // Convertir la sección "Datos" en una lista de objetos CLISTA_DETALLE
                string datosJson = JsonConvert.SerializeObject(jsonData.Datos);
                List<ProductoBEAPI> lista = JsonConvert.DeserializeObject<List<ProductoBEAPI>>(datosJson);

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar la respuesta del API: " + ex.Message);
                return new List<ProductoBEAPI>(); // Retorna lista vacía en caso de error
            }
        }

        #endregion

        #region Unidad de medida
        [GenerateNonce]
        public ActionResult UnidadesMant()
        {
            Session["NavIndex"] = "4";

            return View();
        }

        [HttpPost]
        public JsonResult ListarUnidadesMedida(string vchUnidadMedida, string start, string length, int draw)
        {
            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            var response = new MantenimientosBL().ListarUnidadMedida(vchUnidadMedida, start, length, order);
            List<UnidadMedidaBE> lista = new List<UnidadMedidaBE>();
            lista = (List<UnidadMedidaBE>)response.Entity;
            //var pagedData = lista.Skip(0).Take(10).ToList();
            var totalData = response.count;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = lista });

            //var json = Json(new { data = List });
            //return json;
        }
        [HttpPost]
        public JsonResult CreateUnidadMedida(UnidadMedidaBE obj)
        {
            var response = new MantenimientosBL().CreateUnidadMedida(obj);
            return Json(response);
        }
        [HttpPost]
        public JsonResult GetUnidadMedida(int id)
        {
            var response = new MantenimientosBL().GetUnidadMedida(id);
            return Json(response);
        }
        [HttpPost]
        public JsonResult UpdateUnidadMedida(UnidadMedidaBE obj)
        {
            var response = new MantenimientosBL().UpdateUnidadMedida(obj);
            return Json(response);
        }


        #endregion

        #region Mantenimiento_TipoMoneda
        [GenerateNonce]
        public ActionResult TipoMonedaMant()
        {
            Session["NavIndex"] = "4";
            return View();
        }

        public JsonResult ListarTipoMonedas(string codMoneda, string dscMoneda, string activo)
        {
            MantenimientosBL response = new MantenimientosBL();
            List<TipoMonedaBE> List = response.ListarTipoMonedas(codMoneda,dscMoneda, activo);
            var json = Json(new { data = List });
            return json;

        }

        [HttpPost]
        public JsonResult ActivarInactivarTipoMoneda(int idMoneda, bool flgActivo)
        {
            MantenimientosBL bl = new MantenimientosBL();
            Response response = bl.ActivarInactivarTipoMoneda(idMoneda, flgActivo);
            return Json(response);
        }

        [HttpPost]
        public JsonResult InsertarTipoMoneda(string codMoneda, string dscMoneda)
        {
            if(string.IsNullOrWhiteSpace(codMoneda) || string.IsNullOrWhiteSpace(dscMoneda))
            {
                return Json(new Response { HUBO_ERROR = true, MENSAJE_ERROR = "Codigo o descripcion vacios ingresa datos p, eres o te haces?" });
            }

            MantenimientosBL bl = new MantenimientosBL();
            Response response = bl.InsertarTipoMoneda(codMoneda.Trim(), dscMoneda.Trim());
            return Json(response);
        }

        [HttpPost]
        public JsonResult EditarTipoMoneda(int idMoneda, string dscMoneda,bool flgActivo)
        {
            if (string.IsNullOrWhiteSpace(dscMoneda))
            {
                return Json(new Response { HUBO_ERROR = true, MENSAJE_ERROR = "La descripcion se encuentra vacia p" });
            }

            MantenimientosBL bl = new MantenimientosBL();
            Response response = bl.EditarTipoMoneda(idMoneda, dscMoneda.Trim(), flgActivo);
                return Json(response);
        }

        [HttpPost]
        public JsonResult ObtenerTipoMonedaId(int idTipoMoneda)
        {
            var response = new MantenimientosBL().ObtenerTipoMoneda(idTipoMoneda);
            return Json(response);
        }

        #endregion

    }
}