using BE;
using BL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json.Linq;
using System.IO;
using OfficeOpenXml;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class ReportesController : Controller
    {
        // GET: Reportes
        [GenerateNonce]
        public ActionResult Reporte_Detalle()
        {
            return View();
        }

        [GenerateNonce]
        public ActionResult Reporte_Lectura()
        {
            return View();
        }

        [GenerateNonce]
        public ActionResult Reporte_Conteo()
        {
            return View();
        }

        [GenerateNonce]
        public ActionResult Historico_Inventariado()
        {
            Session["NavIndex"] = "3";
            return View();
        }
        public ActionResult FillCbxInventario()
        {
            CombosBE objCombo = new CombosBE();
            objCombo.intValue = -1;
            objCombo.vchdesc = "Seleccionar Almacen";
            List<CombosBE> listInventarios = new List<CombosBE>();
            listInventarios = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listInventarios.Insert(0, objCombo);
            //ViewBag.ListInventarios = listInventarios;
            return Json(listInventarios);
        }
        public JsonResult TablaHistorico(int ID_Almacen, string filtrarFecha, string dtmIni, string dtmFin)
        {
            ReportesBL response = new ReportesBL();
            DataTable dt = new DataTable();
            dt = response.TablaHistorico(ID_Almacen, filtrarFecha, dtmIni, dtmFin).Entity as DataTable;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return Json(json);
        }

        public JsonResult TablaDetalle(string cod_Inventario, int NConteo)
        {
            ReportesBL response = new ReportesBL();
            DataTable dt = new DataTable();
            dt = response.TablaDetalle(cod_Inventario, NConteo).Entity as DataTable;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return Json(json);
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

        public JsonResult TablaDetalle_WEB(string cod_Inventario, int NConteo, string start, string length, string searchValue, int draw)
        {
            //ReportesBL response = new ReportesBL();
            //DataTable dt = new DataTable();
            //dt = response.TablaDetalle(cod_Inventario, NConteo).Entity as DataTable;
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            //return Json(json);

            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            Response response = new ReportesBL().TablaDetalle_WEB(cod_Inventario, NConteo, start, length, order, searchValue);
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject();
            var jsondata = ConvertDataTableToList(response.Entity as DataTable);
            var totalData = response.count;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = jsondata });

        }
        public ActionResult TablaLecturas(string cod_Inventario, int NConteo)
        {
            ReportesBL response = new ReportesBL();
            DataTable dt = new DataTable();
            dt = response.TablaLecturas(cod_Inventario, NConteo).Entity as DataTable;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            return Json(json);

        }


        public ActionResult Tabla_DET_LEC_Excel(string cod_Inventario, int NConteo, string titulo, int tipo, string Almacen)
        {
            //ReportesBL response = new ReportesBL();
            //DataTable dt = new DataTable();
            //dt = response.TablaLecturas(cod_Inventario, NConteo).Entity as DataTable;
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            //return Json(json);
            Response response = new Response();
            
            if (tipo == 1) {
                response = new ReportesBL().TablaDetalle(cod_Inventario, NConteo);
            }
            else{
                response = new ReportesBL().TablaLecturas(cod_Inventario, NConteo);
            }
                
            
            // Obtener los datos del DataTable
            DataTable dt = (DataTable)response.Entity;
            DataTable dtNew = FiltrarDatatable(tipo, dt);
            MemoryStream ms = new MemoryStream();
            string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");
            using (FileStream fs = System.IO.File.OpenRead(plantilla))
            using (ExcelPackage excelPackage = new ExcelPackage(fs))
            {
                ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                // Agregar título en la celda D3
                //string _titulo = titulo;

                //decimal suma1 = 0;
                //decimal suma2 = 0;
                //decimal suma3 = 0;
                DataRow FilaVacia = dtNew.NewRow();
                dtNew.Rows.Add(FilaVacia);
                
                //titulo = "Detalle Inventario " + " - Conteo " + NConteo;
              
                excelWorksheet.Cells["D3"].Value = titulo;
                excelWorksheet.Cells["E8"].Value = Almacen;
                excelWorksheet.Cells["G8"].Value = cod_Inventario;

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
                FileDownloadName = ""+titulo + ".xlsx"
            };


        }
      
        public DataTable FiltrarDatatable(int tipoReporte,  DataTable dt)
        {
            DataTable dt_Ori = new DataTable();
            dt_Ori = dt;
            DataTable dt_New = new DataTable();
            List<string> listColumn = null;

            if (tipoReporte == 1)
            {
                listColumn = new List<string>() { "COD_UBICACION", "DSC_UBICACION", "COD_PRODUCTO", "DSC_PRODUCTO", "LOTE_PRODUCTO", "STOCK_INICIAL" };
                //IQFARMA
                //listColumn = new List<string>() { "COD_UBICACION", "DSC_UBICACION", "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "LOTE_PRODUCTO", "STOCK_INICIAL" };
                //FIN IQFARMA
            }
            else {
                listColumn = new List<string>() { "FCH_REGISTRO_TERMINAL", "COD_PRODUCTO", "DSC_PRODUCTO", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO", "ID_TERMINAL", "DSC_UBICACION" };
                //IQFARMA
                //listColumn = new List<string>() { "FCH_REGISTRO_TERMINAL", "COD_PRODUCTO", "DSC_PRODUCTO", "DSC_ANALISIS", "DSC_OBSERVACION", "DSC_BALANZA", "LOTE_PRODUCTO", "STOCK_INVENTARIADO", "COD_USUARIO_REGISTRO", "ID_TERMINAL", "DSC_UBICACION" };
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

    }
}