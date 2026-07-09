using BE;
using BL;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Text.Json;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class InventarioController : Controller
    {
        [GenerateNonce]
        public ActionResult PrepararInventario()
        {
            return View("CrearInventario/PrepararInventario");
        }

        [GenerateNonce]
        public ActionResult GestionInventario()
        {
            ViewBag.reloadTable = ConfigurationManager.AppSettings["CargaGestorInventario"].ToString();

            return View("GestorInventario/GestionInventario");
        }

        // GET: Inventario
        [GenerateNonce]

        public ActionResult InventarioPrincipal()
        {
            Session["NavIndex"] = "1";

            //CombosBE objCombo = new CombosBE();
            //objCombo.intValue = -1;
            //objCombo.vchdesc = "Seleccionar Inventario";
            //List<CombosBE> listInventarios = new List<CombosBE>();
            //listInventarios = (List<CombosBE>)new CombosBL().cbxInventario().Entity;
            //listInventarios.Insert(0, objCombo);
            //ViewBag.ListInventarios = listInventarios;
            //List<CombosBE> listAlmacenes = new List<CombosBE>();
            //listAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            //ViewBag.ListaAlmacenes = listAlmacenes;
            ViewBag.reloadTable = ConfigurationManager.AppSettings["CargaGestorInventario"].ToString();

            List<CombosBE> listAlmacenes = new List<CombosBE>();
            listAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            ViewBag.ListaAlmacenes = listAlmacenes;

            return View();
        }

        public ActionResult FillCbxInventario()
        {
            CombosBE objCombo = new CombosBE();
            objCombo.intValue = -1;
            objCombo.vchdesc = "Seleccionar Inventario";
            List<CombosBE> listInventarios = new List<CombosBE>();
            listInventarios = (List<CombosBE>)new CombosBL().cbxInventario().Entity;
            listInventarios.Insert(0, objCombo);
            //ViewBag.ListInventarios = listInventarios;
            return Json(listInventarios);
        }

        public ActionResult FillCbxAlmacenInv()
        {
            List<CombosBE> listAlmacenes = new List<CombosBE>();
            listAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            //ViewBag.ListInventarios = listInventarios;
            return Json(listAlmacenes);
        }

        public ActionResult FillCbxUbicacionInv(int id_almacen)
        {
            List<CombosBE> list = new List<CombosBE>();
            list = (List<CombosBE>)new CombosBL().cbxUbicacion(id_almacen).Entity;

            return new JsonResult
            {
                Data = list,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public ActionResult FillCbxProductoInv(string dsc_prod)
        {
            List<CombosBE> list = new List<CombosBE>();
            list = (List<CombosBE>)new CombosBL().cbxProducto(dsc_prod).Entity;
            //ViewBag.ListInventarios = listInventarios;
            return Json(list);
        }



        [HttpPost]
        public JsonResult ListarInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3, int start, int length, int draw, string searchValue)
        {

            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            //var search = Request.Form["search[value]"].FirstOrDefault();


            var response = new InventarioBL().ListarInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3, start.ToString(), length.ToString(), order, searchValue);
            List<TblInventarioBE> lista = new List<TblInventarioBE>();
            lista = (List<TblInventarioBE>)response.Entity;
            // var pagedData = lista.Skip(0).Take(10).ToList();
            var totalData = response.count;
            List<decimal> Listfooter = new List<decimal>();
            Listfooter = (List<decimal>)response.footerTable;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = lista, Listfooter = Listfooter });



            //InventarioBL response = new InventarioBL();
            //List<TblInventarioBE> List = response.ListarInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3,start.ToString(), length.ToString(), order);
            //var json = Json(new { data = List });
            //return json;
        }

        [HttpPost]
        public JsonResult GetInventario(string idEmpresa, int Almacen, string CodInventario, string CodEstado, string flg_filtroFecha, string fch_inicio, string fch_fin)
        {
            var response = new InventarioBL().GetInventario(idEmpresa, Almacen, CodInventario, CodEstado, flg_filtroFecha, fch_inicio, fch_fin);

            var lista = new List<object>();
            DataTable dt = response.Entity as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new
                {
                    Id = row["ID_INVENTARIO"],
                    Nombre = row["COD_INVENTARIO"],
                    Precio = row["ID_EMPRESA"],
                    COD_EMPRESA = row["COD_EMPRESA"],
                    DSC_EMPRESA = row["DSC_EMPRESA"],
                    FCH_INICIO = row["FCH_INICIO"],
                    FCH_FIN = row["FCH_FIN"],
                    COD_ESTADO = row["COD_ESTADO"],
                    DSC_ESTADO = row["DSC_ESTADO"],
                    DSC_OBSERVACION = row["DSC_OBSERVACION"],
                    FLG_ACTIVO = row["FLG_ACTIVO"],
                    NRO_CONTEO = row["NRO_CONTEO"],
                    ERRORES = row["ERRORES"]
                });
            }

            return Json(lista);
        }

        [HttpPost]
        public JsonResult CerrarIventario(string codInventario, int conteo)
        {
            var response = new InventarioBL().CerrarIventario(codInventario, conteo);
            return Json(response);
        }
        [HttpPost]
        public JsonResult ConteoDiferencial(string codInventario, int conteo)
        {
            var response = new InventarioBL().ConteoDiferencial(codInventario, conteo);
            return Json(response);
        }
        [HttpPost]
        public JsonResult ConteoReinicio(string codInventario, int conteo)
        {
            var response = new InventarioBL().ConteoReinicio(codInventario, conteo);
            return Json(response);
        }
        [HttpPost]
        public JsonResult ListarUsuariosAsociados(string dscAlmacen)
        {
            InventarioBL response = new InventarioBL();
            List<UsuarioBE> UserList = response.ListarUsuariosAsociados(dscAlmacen);
            var json = Json(new { data = UserList });
            return json;
        }

        public ActionResult ExportInventario(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3, string inventario, string codigo)
        {
            var response = new InventarioBL().ExportInventario(COD_INVENTARIO, NRO_CONTEO_1, NRO_CONTEO_2, NRO_CONTEO_3);
            bool CTotalizado = false;
            if (NRO_CONTEO_1 == 1 && NRO_CONTEO_2 == 2 && NRO_CONTEO_3 == 3) { CTotalizado = true; }
            DataTable dt = (DataTable)response.Entity;

            // Crear una nueva instancia de MemoryStream
            MemoryStream ms = new MemoryStream();
            string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");
            using (FileStream fs = System.IO.File.OpenRead(plantilla))
            using (ExcelPackage excelPackage = new ExcelPackage(fs))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                // Agregar título en la celda D3
                string titulo = "";

                DataRow FilaVacia = dt.NewRow();
                dt.Rows.Add(FilaVacia);
                string DatoFiltro = "";
                if (NRO_CONTEO_1 == 1) { DatoFiltro = "1"; }
                if (NRO_CONTEO_2 == 2) { DatoFiltro = "2"; }
                if (NRO_CONTEO_3 == 3) { DatoFiltro = "3"; }
                if (CTotalizado) { DatoFiltro = "Totalizado"; }

                titulo = "" + inventario + " - Conteo " + DatoFiltro;

                excelWorksheet.Cells["D3"].Value = titulo;
                excelWorksheet.Cells["E8"].Value = inventario;
                excelWorksheet.Cells["G8"].Value = codigo;

                // Escribir las cabeceras en el archivo Excel
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    excelWorksheet.Cells[11, i + 1].Value = dt.Columns[i].ColumnName;
                }

                // Escribir los datos en el archivo Excel
                if (dt.Rows.Count > 0)
                {
                    excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                }
                string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                if (System.IO.File.Exists(rutaImagen))
                {
                    //System.Drawing.Image imagen = System.Drawing.Image.FromFile(Server.MapPath(@"~\Assets\IMG\LogoExcel.png"));
                    //// Agregar imagen en la celda A1
                    //var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                    //picture.SetPosition(0, 0); // Posición de la celda A1
                    //picture.SetSize(260, 111); // Tamaño de la imagen (en píxeles)

                    using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                    {
                        // Agregar imagen en la celda A1
                        var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                        picture.SetPosition(0, 0); // Posición de la celda A1
                        picture.SetSize(260, 111); // Tamaño de la imagen (en píxeles)
                    } // La imagen se libera automáticamente al salir del bloque using

                }

                // Guardar el archivo Excel en el MemoryStream
                excelPackage.SaveAs(ms);
            }
            // Establecer la posición del MemoryStream al principio
            ms.Position = 0;
            // Devolver el archivo Excel como un FileStreamResult
            return new FileStreamResult(ms, "application/xlsx")
            {
                FileDownloadName = "Reporte Conteo.xlsx"
            };

        }
        public JsonResult DrawnBarChart(string CodInventario, int tipoGrafico)
        {
            InventarioBL response = new InventarioBL();
            DataTable dt = new DataTable();
            dt = response.DrawnBarChart(CodInventario, tipoGrafico).Entity as DataTable;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return Json(json);
        }

        public JsonResult ReportesInventario_WEB(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario, int draw,
            string start, string length, string searchValue)
        {
            //InventarioBL response = new InventarioBL();
            //DataTable dt = new DataTable();

            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            Response response = new InventarioBL().ReportesInventario_WEB(tipoReporte, Cod_inventario, DatoFiltro, NConteo, Usuario, start, length, order, searchValue);//.Entity as DataTable;
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject();
            var jsondata = ConvertDataTableToList(response.Entity as DataTable);
            var totalData = response.count;
            List<decimal> Listfooter = new List<decimal>();
            Listfooter = (List<decimal>)response.footerTable;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = jsondata, Listfooter = Listfooter });

        }

        public JsonResult ReportesInventario(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario)
        {
            InventarioBL response = new InventarioBL();
            DataTable dt = new DataTable();
            dt = response.ReportesInventario(tipoReporte, Cod_inventario, DatoFiltro, NConteo, Usuario).Entity as DataTable;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return Json(json);
        }

        public ActionResult ExportarExcel(int tipoReporte, string Cod_inventario, string DatoFiltro, string NConteo, string Usuario, string inventario, string codigo)
        {
            var response = new InventarioBL().ReportesInventario(tipoReporte, Cod_inventario, DatoFiltro, NConteo, Usuario);

            // Obtener los datos del DataTable
            DataTable dt = (DataTable)response.Entity;

            DataTable dtNew = FiltrarDatatable(tipoReporte, DatoFiltro, dt);
            // Crear una nueva instancia de MemoryStream
            MemoryStream ms = new MemoryStream();

            // Ruta de la plantilla del archivo Excel
            string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

            // Cargar la plantilla del archivo Excel
            using (FileStream fs = System.IO.File.OpenRead(plantilla))
            using (ExcelPackage excelPackage = new ExcelPackage(fs))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                // Agregar título en la celda D3
                string titulo = "";

                decimal suma1 = 0;
                decimal suma2 = 0;
                decimal suma3 = 0;
                DataRow FilaVacia = dtNew.NewRow();
                dtNew.Rows.Add(FilaVacia);
                switch (tipoReporte)
                {
                    case 0:
                        titulo = "Reporte Diferencial Conteo - " + DatoFiltro;
                        suma1 = dtNew.AsEnumerable()
                                 .Sum(row =>
                                 {
                                     var valor = row.Field<object>("STOCK_INICIAL");
                                     return (valor != DBNull.Value && valor != null) ? Convert.ToDecimal(valor) : 0;
                                 });
                        var columna = "";
                        if (DatoFiltro == "1") { columna = "CONTEO_UNO"; }
                        if (DatoFiltro == "2") { columna = "CONTEO_DOS"; }
                        if (DatoFiltro == "3") { columna = "CONTEO_TRE"; }
                        suma2 = dtNew.AsEnumerable()
                                 .Sum(row =>
                                 {
                                     var valor = row.Field<object>(columna);
                                     return (valor != DBNull.Value && valor != null) ? Convert.ToDecimal(valor) : 0;
                                 });

                        suma3 = dtNew.AsEnumerable()
                                 .Sum(row =>
                                 {
                                     var valor = row.Field<object>("STOCK_DIFERENCIAL");
                                     return (valor != DBNull.Value && valor != null) ? Convert.ToDecimal(valor) : 0;
                                 });
                        DataRow nuevaFila1 = dtNew.NewRow(); DataRow nuevaFila2 = dtNew.NewRow(); DataRow nuevaFila3 = dtNew.NewRow();
                        nuevaFila1["LOTE_PRODUCTO"] = "Suma Inicial";
                        nuevaFila1["STOCK_INICIAL"] = suma1;
                        nuevaFila2["LOTE_PRODUCTO"] = "Suma Final";
                        nuevaFila2["STOCK_INICIAL"] = suma2;
                        nuevaFila3["LOTE_PRODUCTO"] = "Diferencial";
                        nuevaFila3["STOCK_INICIAL"] = suma3;
                        dtNew.Rows.Add(nuevaFila1); dtNew.Rows.Add(nuevaFila2); dtNew.Rows.Add(nuevaFila3);

                        break;
                    case 1:
                        titulo = "Reporte de Productos";
                        break;
                    case 2:
                        titulo = "Reporte de Ubicaciones Conteo - " + NConteo;
                        break;
                    case 3:
                        titulo = "Reporte de Lecturas de Usuario";
                        break;
                }

                if (tipoReporte != 0)
                {

                    suma1 = dtNew.AsEnumerable()
                                .Sum(row =>
                                {
                                    var valor = row.Field<object>("STOCK_INVENTARIADO");
                                    return (valor != DBNull.Value && valor != null) ? Convert.ToDecimal(valor) : 0;
                                });

                    DataRow nuevaFila1 = dtNew.NewRow();
                    nuevaFila1["LOTE_PRODUCTO"] = "Total";
                    nuevaFila1["STOCK_INVENTARIADO"] = suma1;
                    dtNew.Rows.Add(nuevaFila1);
                }
                excelWorksheet.Cells["D3"].Value = titulo;
                excelWorksheet.Cells["E8"].Value = inventario;
                excelWorksheet.Cells["G8"].Value = codigo;

                // Escribir las cabeceras en el archivo Excel
                for (int i = 0; i < dtNew.Columns.Count; i++)
                {
                    excelWorksheet.Cells[11, i + 1].Value = dtNew.Columns[i].ColumnName;
                }

                // Escribir los datos en el archivo Excel
                if (dtNew.Rows.Count > 0)
                {
                    excelWorksheet.Cells["A12"].LoadFromDataTable(dtNew, false);
                }
                string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                if (System.IO.File.Exists(rutaImagen))
                {
                    //System.Drawing.Image imagen = System.Drawing.Image.FromFile(Server.MapPath(@"~\Assets\IMG\LogoExcel.png"));

                    //// Agregar imagen en la celda A1
                    //var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                    //picture.SetPosition(0, 0); // Posición de la celda A1
                    //picture.SetSize(260, 111); // Tamaño de la imagen (en píxeles)
                    using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                    {
                        // Agregar imagen en la celda A1
                        var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                        picture.SetPosition(0, 0); // Posición de la celda A1
                        picture.SetSize(260, 111); // Tamaño de la imagen (en píxeles)
                    } // La imagen se libera automáticamente al salir del bloque using
                }

                // Guardar el archivo Excel en el MemoryStream
                excelPackage.SaveAs(ms);
            }
            // Establecer la posición del MemoryStream al principio
            ms.Position = 0;
            // Devolver el archivo Excel como un FileStreamResult
            return new FileStreamResult(ms, "application/xlsx")
            {
                FileDownloadName = "Reporte de Verificación.xlsx"
            };
        }

        public DataTable FiltrarDatatable(int tipoReporte, string DatoFiltro, DataTable dt)
        {
            DataTable dt_Ori = new DataTable();
            dt_Ori = dt;
            DataTable dt_New = new DataTable();
            List<string> listColumn = null;
            if (tipoReporte == 0)
            {
                string NConteo = "";
                switch (DatoFiltro)
                {
                    case "1": NConteo = "CONTEO_UNO"; break;
                    case "2": NConteo = "CONTEO_DOS"; break;
                    case "3": NConteo = "CONTEO_TRE"; break;
                }

                listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_UBICACION", "LOTE_PRODUCTO", "STOCK_INICIAL", NConteo, "STOCK_DIFERENCIAL" };
                //IQFARMA
                //listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "DSC_UBICACION", "LOTE_PRODUCTO", "STOCK_INICIAL", NConteo, "STOCK_DIFERENCIAL"};
                //FIN IQFARMA
            }
            if (tipoReporte == 1)
            {
                listColumn = new List<string>() { "DSC_PRODUCTO", "COD_UBICACION", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO" };
                //IQFARMA
                //listColumn = new List<string>() { "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "COD_UBICACION", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO" };

                //FIN IQFARMA


            }
            if (tipoReporte == 2)
            {
                listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO" };
                //IQFARMA
                //listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO" };

                //FIN IQFARMA
            }
            if (tipoReporte == 3)
            {
                listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "COD_UBICACION", "LOTE_PRODUCTO", "STOCK_INVENTARIADO" };
                //IQFARMA
                //listColumn = new List<string>() { "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "COD_UBICACION", "LOTE_PRODUCTO", "STOCK_INVENTARIADO" };

                //FIN IQFARMA
            }

            foreach (string nombreColumna in listColumn)
            {
                if (dt_Ori.Columns.Contains(nombreColumna))
                {
                    dt_New.Columns.Add(nombreColumna, dt_Ori.Columns[nombreColumna].DataType);
                }
            }

            // Copiar los datos del DataTable original al nuevo DataTable
            foreach (DataRow filaOriginal in dt_Ori.Rows)
            {
                DataRow filaNueva = dt_New.NewRow();
                foreach (DataColumn columna in dt_New.Columns)
                {
                    if (dt_Ori.Columns.Contains(columna.ColumnName))
                    {
                        filaNueva[columna.ColumnName] = filaOriginal[columna.ColumnName];
                    }
                }
                dt_New.Rows.Add(filaNueva);
            }


            return dt_New;
        }
        [HttpPost]
        public ActionResult ValidarBloque1(string Cod_inventario, int Id_Almacen)
        {
            var response = new InventarioBL().ValidarDatosInventario(Cod_inventario, Id_Almacen);
            return Json(response);
        }
        // YA NO ENVIA string xmlData
        public ActionResult InsertInv_InvDetalle(Guid importacionId, string CodInv, int Id_Almacen)
        {
            //System.Diagnostics.Debug.WriteLine("Entró al método");
            var response = new InventarioBL().InsertInv_InvDetalle(importacionId, Session["UserName"].ToString(), CodInv, Id_Almacen);
            return Json(response);
            //return Json(new
            //{
            //    xml = xmlData?.Length,
            //    cod = CodInv,
            //    almacen = Id_Almacen
            //});
        }

        private List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }

            return list;
        }

        public ActionResult CargarAlmacenesAPIExterna(string COD_ALM)
        {
            UTIL.APIExterna api = new UTIL.APIExterna();
            BL.InventarioBL BLresponse = new BL.InventarioBL();
            Response response = BLresponse.GetAPI_StockALM();
            string URLAPI;

            if (response.ERR_CODE == 0) { URLAPI = response.MENSAJE_ERROR; }
            else { return Json(response); }

            var ApiResponse = api.ObtenerSTOCK_ALM_IQFARMA(URLAPI, COD_ALM);
            if (ApiResponse.Contains("\"Datos\":null"))
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se obtuvo respuesta de la API o la lista se encuetra vacia";
                return Json(response);
            }

            bool huboError;
            string mensaje;

            // CREAR EL INVENTARIO Y OBTENER EL ID
            int? idInventario = BLresponse.CLI_SP_CREAR_INVENTARIO(COD_ALM, out huboError, out mensaje);

            if (huboError)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = mensaje;
                return Json(response);

            }
            else
            {
                // INSERT DETALLE INVENTARIO
                Import_Inventario_DetalleInventario(ApiResponse, (int)idInventario);

                response.MENSAJE_ERROR = "Inventario Creado con Exito";
                return Json(response);
            }

            return Json(response);
        }
        public Response Import_Inventario_DetalleInventario(string xmlResponse, int idInventario)
        {
            Response response = new Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            try
            {
                List<CLISTA_DETALLEAPI> listaDetalle = Convert_XML_List(xmlResponse);

                if (listaDetalle == null || listaDetalle.Count == 0)
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "No se pudieron obtener datos de la respuesta XML o la lista está vacía.";
                    return response;
                }

                var dataAccess = new InventarioBL();

                // Procesar la lista en bloques
                foreach (List<CLISTA_DETALLEAPI> listaBloque in SplitListIntoChunks(listaDetalle, 100))
                {
                    string xmlDataBlock = ConvertListToXml(listaBloque);
                    dataAccess.ImportarDetInventario_Inventario(xmlDataBlock, "API", idInventario);
                }

                dataAccess.Insert_ASF_DETALLE_INVENTARIO("API");

                response.Entity = ListaResult;
                return response;
            }
            catch (Exception ex)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "Ocurrió un error: " + ex.Message;
                return response;
            }
        }



        private List<List<CLISTA_DETALLEAPI>> SplitListIntoChunks(List<CLISTA_DETALLEAPI> lista, int chunkSize)
        {
            return lista.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        public static List<CLISTA_DETALLEAPI> Convert_XML_List(string xmlResponse)
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
                List<CLISTA_DETALLEAPI> lista = JsonConvert.DeserializeObject<List<CLISTA_DETALLEAPI>>(datosJson);

                return lista;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar la respuesta del API: " + ex.Message);
                return new List<CLISTA_DETALLEAPI>(); // Retorna lista vacía en caso de error
            }
        }

        private string ConvertListToXml(List<CLISTA_DETALLEAPI> lista)
        {
            XDocument xmlDocument = new XDocument(
                new XElement("Root",
                    lista.ConvertAll(item =>
                        new XElement("Item",
                            new XElement("COD_ALMACEN", item.COD_ALMACEN),
                            new XElement("COD_UBICACION", item.COD_UBICACION),
                            new XElement("COD_PRODUCTO", item.COD_PRODUCTO),
                            new XElement("DSC_PRODUCTO", item.DSC_PRODUCTO),
                            new XElement("DSC_ANALISIS", item.DSC_ANALISIS),
                            new XElement("DSC_OBSERVACION", item.DSC_OBSERVACION),
                            new XElement("DSC_BALANZA", item.DSC_BALANZA),
                            new XElement("LOTE_PRODUCTO", item.LOTE_PRODUCTO),
                            new XElement("STOCK_ACTUAL", Convert.ToInt32(item.STOCK))
                        )
                    )
                )
            );

            return xmlDocument.ToString();
        }


        //[HttpPost]
        //public JsonResult ExportConteoTotalizado(string COD_INVENTARIO, int NRO_CONTEO_1, int NRO_CONTEO_2, int NRO_CONTEO_3)
        //{

        //}
    }
}