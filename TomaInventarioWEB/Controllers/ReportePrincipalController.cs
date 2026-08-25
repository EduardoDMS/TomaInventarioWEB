using BE;
using BE.Reportes;
using BL;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class ReportePrincipalController : Controller
    {
        // ==================================================
        // NUEVOS MÉTODOS DE REPORTES
        // ==================================================
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

        // DIFERENCIAL
        [GenerateNonce]
        public ActionResult Reporte_Diferencial()
        {
            //Session["NavIndex"] = "2";

            // Cargar combo de almacenes 
            CombosBE objComboAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteDiferencial(string codInventario, string estado, string tipoDiferencia, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteDiferencial(codInventario, estado, tipoDiferencia, busqueda);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelDiferencial(string almacen, string codInventario, string estado, string tipoDiferencia, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteDiferencial(codInventario, estado, tipoDiferencia, busqueda);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteDiferencialBE reporte = (ReporteDiferencialBE)response.Entity;
                List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE DIFERENCIAL";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.EstadoInventario;
                    excelWorksheet.Cells["E7"].Value = reporte.ConteoActual;

                    // INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Productos totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.ProductosInventariados;

                    //excelWorksheet.Cells["D9"].Value = "Productos sin diferencia";
                    //excelWorksheet.Cells["D10"].Value = reporte.ProductosSinDiferencia;

                    //excelWorksheet.Cells["F9"].Value = "Productos con diferencia";
                    //excelWorksheet.Cells["F10"].Value = reporte.ProductosConDiferencia;

                    //excelWorksheet.Cells["H9"].Value = "Stock sobrante";
                    //excelWorksheet.Cells["H10"].Value = reporte.TotalDeSobrantes;

                    //excelWorksheet.Cells["J9"].Value = "Stock faltante";
                    //excelWorksheet.Cells["J10"].Value = reporte.TotalDeFaltantes;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    "H9:I10",
                    //    "J9:K10"
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "CÓDIGO", "PRODUCTO", "STOCK INICIAL", "STOCK CONTADO", "DIFERENCIA", "ESTADO", "TIPO DIFERENCIA" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:H9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;
                    foreach (var item in reporte.TblReporteDiferencial)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.CodProducto;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.DscProducto;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.StockInicial;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.StockFinal;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.StockDiferencial;
                        excelWorksheet.Cells[filaInicio, 7].Value = item.Estado;
                        excelWorksheet.Cells[filaInicio, 8].Value = item.TipoDiferencia;

                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteDiferencial.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

                    excelWorksheet.Cells[filaTotal, 4].Value = totales[0]; // Stock Inicial
                    excelWorksheet.Cells[filaTotal, 5].Value = totales[1]; // Stock Conteo
                    excelWorksheet.Cells[filaTotal, 6].Value = totales[2]; // Diferencial

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 4].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 6].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 6].Style.Font.Color.SetColor(
                        totales[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                    );


                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 2;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Diferencial_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // CONTEO
        [GenerateNonce]
        public ActionResult Reporte_Conteo()
        {
            //Session["NavIndex"] = "2";

            // Cargar combo de almacenes 
            CombosBE objComboAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteConteo(string codInventario, int nroConteo, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteConteo(codInventario, nroConteo, busqueda);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelConteo(string almacen, string codInventario, int nroConteo, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteConteo(codInventario, nroConteo, busqueda);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteConteoBE reporte = (ReporteConteoBE)response.Entity;
                List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE POR CONTEO";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.EstadoConteo;
                    excelWorksheet.Cells["E7"].Value = reporte.ConteoSeleccionado;

                    // INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Productos totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.ProductosInventariados;

                    ////excelWorksheet.Cells["D9"].Value = "Productos fuera del inventario";
                    ////excelWorksheet.Cells["D10"].Value = reporte.ProductosFueraInventario;

                    //excelWorksheet.Cells["D9"].Value = "Usuarios participantes";
                    //excelWorksheet.Cells["D10"].Value = reporte.UsuariosParticipantes;

                    //excelWorksheet.Cells["F9"].Value = "Duración del conteo";
                    //excelWorksheet.Cells["F10"].Value = reporte.DuracionConteo;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    //"H9:I10",
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "CÓDIGO", "PRODUCTO", "UBICACIÓN INICIAL", "UBICACIÓN CONTADA", "LOTE INICIAL", "LOTE CONTADO", "STOCK INICIAL", "STOCK CONTADO", "DIFERENCIA", "USUARIO" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:K9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;
                    foreach (var item in reporte.TblReporteConteo)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.CodProducto;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.DscProducto;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.UbicacionInicial;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.UbicacionContada;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.LoteInicial;
                        excelWorksheet.Cells[filaInicio, 7].Value = item.LoteContado;
                        excelWorksheet.Cells[filaInicio, 8].Value = item.StockInicial;
                        excelWorksheet.Cells[filaInicio, 9].Value = item.StockContado;
                        excelWorksheet.Cells[filaInicio, 10].Value = item.StockDiferencial;
                        excelWorksheet.Cells[filaInicio, 11].Value = item.Usuario;

                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteConteo.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

                    excelWorksheet.Cells[filaTotal, 8].Value = totales[0]; // Stock Inicial
                    excelWorksheet.Cells[filaTotal, 9].Value = totales[1]; // Stock Conteo
                    excelWorksheet.Cells[filaTotal, 10].Value = totales[2]; // Diferencial

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 9].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 10].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 10].Style.Font.Color.SetColor(
                        totales[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                    );


                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 2;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Conteo_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // USUARIO
        [GenerateNonce]
        public ActionResult Reporte_Usuario()
        {
            //Session["NavIndex"] = "2";

            // Cargar combo de almacenes 
            CombosBE objComboAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteUsuario(string codInventario, int nroConteo, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteUsuario(codInventario, nroConteo, busqueda);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelUsuario(string almacen, string codInventario, int nroConteo, string busqueda)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteUsuario(codInventario, nroConteo, busqueda);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteUsuarioBE reporte = (ReporteUsuarioBE)response.Entity;
                List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE POR USUARIO";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.EstadoConteo;
                    excelWorksheet.Cells["E7"].Value = reporte.ConteoSeleccionado;

                    // INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Productos totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.ProductosInventariados;

                    ////excelWorksheet.Cells["D9"].Value = "Productos fuera del inventario";
                    ////excelWorksheet.Cells["D10"].Value = reporte.ProductosFueraInventario;

                    //excelWorksheet.Cells["D9"].Value = "Usuario con más lecturas";
                    //excelWorksheet.Cells["D10"].Value = reporte.UsuarioMasLecturas;

                    //excelWorksheet.Cells["F9"].Value = "Usuario con menos lecturas";
                    //excelWorksheet.Cells["F10"].Value = reporte.UsuarioMenosLecturas;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    //"H9:I10",
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "USUARIO", "NOMBRE COMPLETO", "PRODUCTOS LECTURADOS", "UBICACIONES LECTURADAS", "STOCK TOTAL LECTURADO", "TIEMPO INICIO", "TIEMPO FINAL", "LECTURAS / MIN", "PARTICIPACIÓN (H:min)" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:J9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;
                    foreach (var item in reporte.TblReporteUsuarios)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.Usuario;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.NombreCompleto;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.ProductosLecturados;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.UbicacionesLecturadas;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.StockTotalContado;
                        excelWorksheet.Cells[filaInicio, 7].Value = item.FechaHoraInicial;
                        excelWorksheet.Cells[filaInicio, 8].Value = item.FechaHoraFinal;
                        excelWorksheet.Cells[filaInicio, 9].Value = item.PromedioLecturas;
                        excelWorksheet.Cells[filaInicio, 10].Value = item.TiempoParticipacion;

                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteUsuarios.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 6].Value = totales[0]; // Stock Conteo

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 6].Style.Font.Bold = true;

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 2;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Usuario_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // PRODUCTO
        [GenerateNonce]
        public ActionResult Reporte_Producto()
        {
            //Session["NavIndex"] = "2";

            // Cargar combo de almacenes 
            CombosBE objComboAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteProducto(string codInventario, string busqueda, int estado, int observacion)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteProducto(codInventario, busqueda, estado, observacion);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = ex.Message
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelProducto(string almacen, string codInventario, string busqueda, int estado, int observacion)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteProducto(codInventario, busqueda, estado, observacion);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteProductoBE reporte = (ReporteProductoBE)response.Entity;
                List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE POR PRODUCTO";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.EstadoConteo;
                    excelWorksheet.Cells["E7"].Value = reporte.ConteoSeleccionado;

                    //// INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Productos totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.ProductosInventariados;

                    //excelWorksheet.Cells["D9"].Value = "Productos sin diferencias";
                    //excelWorksheet.Cells["D10"].Value = reporte.ProductosFueraInventario;

                    //excelWorksheet.Cells["F9"].Value = "Productos con diferencias";
                    //excelWorksheet.Cells["F10"].Value = reporte.ProductosConDiferencia;

                    ////excelWorksheet.Cells["H9"].Value = "Productos fuera del inventario";
                    ////excelWorksheet.Cells["H10"].Value = reporte.ProductosFueraInventario;

                    //excelWorksheet.Cells["H9"].Value = "Productos fuera de ubicación";
                    //excelWorksheet.Cells["H10"].Value = reporte.ProductosUbicacionDiferente;

                    //excelWorksheet.Cells["J9"].Value = "Productos con lote diferente";
                    //excelWorksheet.Cells["J10"].Value = reporte.ProductosLoteDiferente;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    "H9:I10",
                    //    "J9:K10",
                    //    //"L9:M10",
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "CÓDIGO", "PRODUCTO", "UBICACIÓN INICIAL", "UBICACIÓN CONTADA", "LOTE INICIAL", "LOTE CONTADO", "STOCK INICIAL", "STOCK CONTADO", "DIFERENCIA", "OBSERVACIÓN", "USUARIO" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:L9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;
                    foreach (var item in reporte.TblReporteProductos)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.CodProducto;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.DscProducto;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.UbicacionInicial;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.UbicacionContada;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.LoteInicial;
                        excelWorksheet.Cells[filaInicio, 7].Value = item.LoteContado;
                        excelWorksheet.Cells[filaInicio, 8].Value = item.StockInicial;
                        excelWorksheet.Cells[filaInicio, 9].Value = item.StockContado;
                        excelWorksheet.Cells[filaInicio, 10].Value = item.Diferencia;
                        excelWorksheet.Cells[filaInicio, 11].Value = item.Observacion;
                        excelWorksheet.Cells[filaInicio, 12].Value = item.Usuario;

                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteProductos.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

                    excelWorksheet.Cells[filaTotal, 8].Value = totales[0]; // Stock Inicial
                    excelWorksheet.Cells[filaTotal, 9].Value = totales[1]; // Stock Conteo
                    excelWorksheet.Cells[filaTotal, 10].Value = totales[2]; // Diferencial

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 9].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 10].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 10].Style.Font.Color.SetColor(
                        totales[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                    );

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 2;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Producto_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // UBICACION
        [GenerateNonce]
        public ActionResult Reporte_Ubicacion()
        {
            CombosBE objComboAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteUbicacion(string codInventario, string busqueda, string estado, int diferencias)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteUbicacion(codInventario, busqueda, estado, diferencias);

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    mensaje = ex.Message,
                    detalle = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelUbicacion(string almacen, string codInventario, string busqueda, string estado, int diferencias)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteUbicacion(codInventario, busqueda, estado, diferencias);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteUbicacionBE reporte = (ReporteUbicacionBE)response.Entity;
                List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE POR UBICACIÓN";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.Estado_Inventario;
                    excelWorksheet.Cells["E7"].Value = reporte.Conteo_Actual;

                    // INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Ubicaciones totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.Ubicaciones_Totales;

                    //excelWorksheet.Cells["D9"].Value = "Ubicaciones nuevas";
                    //excelWorksheet.Cells["D10"].Value = reporte.Ubicaciones_Nuevas;

                    //excelWorksheet.Cells["F9"].Value = "Ubicaciones completas";
                    //excelWorksheet.Cells["F10"].Value = reporte.Ubicaciones_Completas;

                    //excelWorksheet.Cells["H9"].Value = "Ubicaciones en progreso";
                    //excelWorksheet.Cells["H10"].Value = reporte.Ubicaciones_En_Proceso;

                    //excelWorksheet.Cells["J9"].Value = "Ubicaciones no iniciadas";
                    //excelWorksheet.Cells["J10"].Value = reporte.Ubicaciones_No_Iniciadas;

                    //excelWorksheet.Cells["L9"].Value = "Top 3 mayor diferencia";
                    //excelWorksheet.Cells["L10"].Value = reporte.Top3_Mayor_Diferencia;

                    //excelWorksheet.Cells["N9"].Value = "Top 3 menor diferencia";
                    //excelWorksheet.Cells["N10"].Value = reporte.Top3_Menor_Diferencia;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    "H9:I10",
                    //    "J9:K10",
                    //    "L9:M10",
                    //    "N9:O10",
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "CÓDIGO", "UBICACIÓN", "PRODUCTOS TOTALES", "STOCK INICIAL", "PRODUCTOS CONTADOS", "STOCK CONTADO", "DIFERENCIA", "ESTADO" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:I9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;
                    foreach (var item in reporte.TblReporteUbicacion)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.Cod_Ubicacion;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.Descripcion_Ubicacion;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.Productos_Totales;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.Stock_Inicial;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.Productos_Contados;
                        excelWorksheet.Cells[filaInicio, 7].Value = item.Stock_Contado;
                        excelWorksheet.Cells[filaInicio, 8].Value = item.Diferencia;
                        excelWorksheet.Cells[filaInicio, 9].Value = item.Estado;

                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteUbicacion.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

                    excelWorksheet.Cells[filaTotal, 4].Value = totales[0]; // Productos total
                    excelWorksheet.Cells[filaTotal, 5].Value = totales[1]; // Stock Inicial
                    excelWorksheet.Cells[filaTotal, 7].Value = totales[2]; // Stock Conteo
                    excelWorksheet.Cells[filaTotal, 8].Value = totales[3]; // Diferencial

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 4].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                        totales[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                    );

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 3;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Producto_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // AUDITORIA
        [GenerateNonce]
        public ActionResult Reporte_Auditoria()
        {
            CombosBE objAlmacen = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objAlmacen);

            ViewBag.ListaAlmacenes = listaAlmacenes;
            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteAuditoria(string codInventario, string busqueda, string estado)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteAuditoria(codInventario, busqueda, estado);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    mensaje = ex.Message,
                    detalle = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        [HttpPost]
        public ActionResult ExportarExcelAuditoria(string almacen, string codInventario, string busqueda, string estado)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteAuditoria(codInventario, busqueda, estado);

                if (response == null ||
                    response.HUBO_ERROR ||
                    response.Entity == null)
                {
                    throw new Exception(
                        response?.MENSAJE_ERROR ??
                        "Error al obtener los datos del reporte"
                    );
                }

                // Obtener datos
                ReporteAuditoriaBE reporte = (ReporteAuditoriaBE)response.Entity;
                //FooterAuditoriaBE footer = (FooterAuditoriaBE)reporte.Footer;

                //List<decimal> totales = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();

                // Generar Excel
                MemoryStream ms = new MemoryStream();

                string plantilla = Server.MapPath(
                    @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
                );

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


                    // LOGO - FILA 1
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetSize(240, 100);
                            picture.SetPosition(1, 5, 1, 5);
                        }
                    }

                    // TITULO - FILA 2
                    excelWorksheet.Cells["D2"].Value = "REPORTE DE AUDITORÍA";

                    // INFORMACIÓN DEL INVENTARIO - FILA 4
                    excelWorksheet.Cells["E4"].Value = almacen;
                    excelWorksheet.Cells["E5"].Value = codInventario;
                    excelWorksheet.Cells["E6"].Value = reporte.Estado_Inventario;
                    excelWorksheet.Cells["E7"].Value = reporte.Conteo_Actual;

                    // INDICADORES DEL REPORTE - FILA 9
                    //excelWorksheet.Cells["B9"].Value = "Productos totales";
                    //excelWorksheet.Cells["B10"].Value = reporte.Productos_Inventariados;

                    //excelWorksheet.Cells["D9"].Value = "Productos sin diferencias";
                    //excelWorksheet.Cells["D10"].Value = reporte.Productos_Sin_Diferencia;

                    //excelWorksheet.Cells["F9"].Value = "Productos con diferencias";
                    //excelWorksheet.Cells["F10"].Value = reporte.Productos_Con_Diferencia;

                    //excelWorksheet.Cells["H9"].Value = "Usuarios participantes";
                    //excelWorksheet.Cells["H10"].Value = reporte.Usuarios_Participantes;

                    //excelWorksheet.Cells["J9"].Value = "Primera lectura";
                    //excelWorksheet.Cells["J10"].Value = reporte.Primera_Lectura;

                    //excelWorksheet.Cells["L9"].Value = "Última lectura";
                    //excelWorksheet.Cells["L10"].Value = reporte.Ultima_Lectura;

                    //// Bordes de los indicadores
                    //string[] rangosKPI =
                    //{
                    //    "B9:C10",
                    //    "D9:E10",
                    //    "F9:G10",
                    //    "H9:I10",
                    //    "J9:K10",
                    //    "L9:M10",
                    //};

                    //foreach (string rango in rangosKPI)
                    //{
                    //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    //}

                    // CABECERA DE TABLA - FILA 12
                    string[] headers = { "CÓDIGO", "PRODUCTO", "STOCK INICIAL", "UBICACION INICIAL", "LOTE INICIAL", "CONTEO 1 STOCK", "CONTEO 1 UBICACIÓN", "CONTEO 1 LOTE", "CONTEO 1 USUARIO", "CONTEO 1 HORA", "CONTEO 2 STOCK", "CONTEO 2 UBICACIÓN", "CONTEO 2 LOTE", "CONTEO 2 USUARIO", "CONTEO 2 HORA", "CONTEO 3 STOCK", "CONTEO 3 UBICACIÓN", "CONTEO 3 LOTE", "CONTEO 3 USUARIO", "CONTEO 3 HORA", "STOCK FINAL", "DIFERENCIA", "ESTADO" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = excelWorksheet.Cells[9, i + 2];
                        cell.Value = headers[i];
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(
                            System.Drawing.ColorTranslator.FromHtml("#305496")
                        );
                    }

                    excelWorksheet.Cells["B9:X9"].AutoFilter = true;

                    // CONTENIDO DE TABLA - FILA 13
                    int filaInicio = 10;

                    foreach (var item in reporte.TblReporteAuditoria)
                    {
                        excelWorksheet.Cells[filaInicio, 2].Value = item.Codigo;
                        excelWorksheet.Cells[filaInicio, 3].Value = item.Producto;
                        excelWorksheet.Cells[filaInicio, 4].Value = item.Stock_Inicial;
                        excelWorksheet.Cells[filaInicio, 5].Value = item.Ubicacion_Inicial;
                        excelWorksheet.Cells[filaInicio, 6].Value = item.Lote_Inicial;

                        // Conteos
                        for (int i = 0; i < item.Conteos.Count; i++)
                        {
                            var conteo = item.Conteos[i];

                            int columnaInicio = 7 + (i * 5);

                            excelWorksheet.Cells[filaInicio, columnaInicio].Value = conteo.Stock_Contado;
                            excelWorksheet.Cells[filaInicio, columnaInicio + 1].Value = conteo.Ubicaciones_Contadas;
                            excelWorksheet.Cells[filaInicio, columnaInicio + 2].Value = conteo.Lote_Contado;
                            excelWorksheet.Cells[filaInicio, columnaInicio + 3].Value = conteo.Usuario;
                            excelWorksheet.Cells[filaInicio, columnaInicio + 4].Value = conteo.Hora_Registro;
                        }

                        excelWorksheet.Cells[filaInicio, 22].Value = item.Stock_Final;
                        excelWorksheet.Cells[filaInicio, 23].Value = item.Diferencia;
                        excelWorksheet.Cells[filaInicio, 24].Value = item.Estado;


                        filaInicio++;
                    }

                    // FOOTER TOTALES
                    int filaTotal = 10 + reporte.TblReporteAuditoria.Count + 1;
                    excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
                    excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

                    excelWorksheet.Cells[filaTotal, 4].Value = reporte.Footer.StockInicialTotal; // Stock Inicial Total

                    // Totales por conteo
                    for (int i = 0; i < reporte.Footer.TotalesPorConteo.Count; i++)
                    {
                        var totalConteo = reporte.Footer.TotalesPorConteo[i];

                        int columnaInicio = 7 + (i * 5);

                        excelWorksheet.Cells[filaTotal, columnaInicio].Value = totalConteo.Total_Stock;
                    }

                    excelWorksheet.Cells[filaTotal, 22].Value = reporte.Footer.StockFinalTotal; // Stock Conteo Total
                    excelWorksheet.Cells[filaTotal, 23].Value = reporte.Footer.StockDiferencial; // Diferencial Total

                    // Formato para totales
                    excelWorksheet.Cells[filaTotal, 4].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 12].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 17].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 22].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 23].Style.Font.Bold = true;
                    excelWorksheet.Cells[filaTotal, 23].Style.Font.Color.SetColor(
                        reporte.Footer.StockDiferencial < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                    );

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
                    for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
                    {
                        excelWorksheet.Column(i).Width += 3;
                    }

                    excelPackage.SaveAs(ms);
                }

                // IMPORTANTE
                ms.Position = 0;

                return new FileStreamResult(
                    ms,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                )
                {
                    FileDownloadName =
                        $"Reporte_Auditoria_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                // Manejo del error
                return new HttpStatusCodeResult(
                    500,
                    ex.Message
                );
            }
        }


        // VALORIZADO
        [GenerateNonce]
        public ActionResult Reporte_Valorizado()
        {
            CombosBE objComboValorizado = new CombosBE();
            List<CombosBE> listaAlmacenes = new List<CombosBE>();
            listaAlmacenes = (List<CombosBE>)new CombosBL().cbxAlmacenes().Entity;
            listaAlmacenes.Insert(0, objComboValorizado);

            ViewBag.ListaAlmacenes = listaAlmacenes;

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerReporteValorizado(string codInventario, string busqueda, int estado)
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerDatosReporteValorizado(codInventario, busqueda, estado);
                return Json(response);
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    error = true,
                    mensaje = ex.Message,
                    detalle = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        
        //          >AQUI ESTA PARA EL EXPORTAR POR EXCEL XD<
        //[HttpPost]
        //public ActionResult ExportarExcelValorizado(string codInventario, string busqueda, int p_impacto)
        //{
        //    try
        //    {
        //        var response = new ReportePrincipalBL().ObtenerDatosReporteValorizado(codInventario, busqueda, p_impacto);

        //        if (response == null ||
        //            response.HUBO_ERROR ||
        //            response.Entity == null)
        //        {
        //            throw new Exception(
        //                response?.MENSAJE_ERROR ??
        //                "Error al obtener los datos del reporte"
        //            );
        //        }

        //        // Obtener datos
        //        ReporteValorizadoBE reporte = (ReporteValorizadoBE)response.Entity;
                
        //        // Generar Excel
        //        MemoryStream ms = new MemoryStream();

        //        string plantilla = Server.MapPath(
        //            @"~\Plantillas\Plantilla_Reportes_Nuevo.xlsx"
        //        );

        //        using (FileStream fs = System.IO.File.OpenRead(plantilla))
        //        using (ExcelPackage excelPackage = new ExcelPackage(fs))
        //        {
        //            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets[1];


        //            // LOGO - FILA 1
        //            string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
        //            if (System.IO.File.Exists(rutaImagen))
        //            {
        //                using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
        //                {
        //                    var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
        //                    picture.SetSize(240, 100);
        //                    picture.SetPosition(1, 5, 1, 5);
        //                }
        //            }

        //            // TITULO - FILA 2
        //            excelWorksheet.Cells["D2"].Value = "REPORTE DE VALORIZADO";

        //            // INFORMACIÓN DEL INVENTARIO - FILA 4
        //            excelWorksheet.Cells["E4"].Value = almacen;
        //            excelWorksheet.Cells["E5"].Value = codInventario;
        //            excelWorksheet.Cells["E6"].Value = reporte.Estado_Inventario;
        //            excelWorksheet.Cells["E7"].Value = reporte.Conteo_Actual;

        //            // INDICADORES DEL REPORTE - FILA 9
        //            //excelWorksheet.Cells["B9"].Value = "Productos totales";
        //            //excelWorksheet.Cells["B10"].Value = reporte.Productos_Inventariados;

        //            //excelWorksheet.Cells["D9"].Value = "Productos sin diferencias";
        //            //excelWorksheet.Cells["D10"].Value = reporte.Productos_Sin_Diferencia;

        //            //excelWorksheet.Cells["F9"].Value = "Productos con diferencias";
        //            //excelWorksheet.Cells["F10"].Value = reporte.Productos_Con_Diferencia;

        //            //excelWorksheet.Cells["H9"].Value = "Usuarios participantes";
        //            //excelWorksheet.Cells["H10"].Value = reporte.Usuarios_Participantes;

        //            //excelWorksheet.Cells["J9"].Value = "Primera lectura";
        //            //excelWorksheet.Cells["J10"].Value = reporte.Primera_Lectura;

        //            //excelWorksheet.Cells["L9"].Value = "Última lectura";
        //            //excelWorksheet.Cells["L10"].Value = reporte.Ultima_Lectura;

        //            //// Bordes de los indicadores
        //            //string[] rangosKPI =
        //            //{
        //            //    "B9:C10",
        //            //    "D9:E10",
        //            //    "F9:G10",
        //            //    "H9:I10",
        //            //    "J9:K10",
        //            //    "L9:M10",
        //            //};

        //            //foreach (string rango in rangosKPI)
        //            //{
        //            //    excelWorksheet.Cells[rango].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        //            //}

        //            // CABECERA DE TABLA - FILA 12
        //            string[] headers = { "CÓDIGO", "PRODUCTO", "STOCK INICIAL", "UBICACION INICIAL", "LOTE INICIAL", "CONTEO 1 STOCK", "CONTEO 1 UBICACIÓN", "CONTEO 1 LOTE", "CONTEO 1 USUARIO", "CONTEO 1 HORA", "CONTEO 2 STOCK", "CONTEO 2 UBICACIÓN", "CONTEO 2 LOTE", "CONTEO 2 USUARIO", "CONTEO 2 HORA", "CONTEO 3 STOCK", "CONTEO 3 UBICACIÓN", "CONTEO 3 LOTE", "CONTEO 3 USUARIO", "CONTEO 3 HORA", "STOCK FINAL", "DIFERENCIA", "ESTADO" };
        //            for (int i = 0; i < headers.Length; i++)
        //            {
        //                var cell = excelWorksheet.Cells[9, i + 2];
        //                cell.Value = headers[i];
        //                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //                cell.Style.Fill.BackgroundColor.SetColor(
        //                    System.Drawing.ColorTranslator.FromHtml("#305496")
        //                );
        //            }

        //            excelWorksheet.Cells["B9:X9"].AutoFilter = true;

        //            // CONTENIDO DE TABLA - FILA 13
        //            int filaInicio = 10;

        //            foreach (var item in reporte.TblReporteAuditoria)
        //            {
        //                excelWorksheet.Cells[filaInicio, 2].Value = item.Codigo;
        //                excelWorksheet.Cells[filaInicio, 3].Value = item.Producto;
        //                excelWorksheet.Cells[filaInicio, 4].Value = item.Stock_Inicial;
        //                excelWorksheet.Cells[filaInicio, 5].Value = item.Ubicacion_Inicial;
        //                excelWorksheet.Cells[filaInicio, 6].Value = item.Lote_Inicial;

        //                // Conteos
        //                for (int i = 0; i < item.Conteos.Count; i++)
        //                {
        //                    var conteo = item.Conteos[i];

        //                    int columnaInicio = 7 + (i * 5);

        //                    excelWorksheet.Cells[filaInicio, columnaInicio].Value = conteo.Stock_Contado;
        //                    excelWorksheet.Cells[filaInicio, columnaInicio + 1].Value = conteo.Ubicaciones_Contadas;
        //                    excelWorksheet.Cells[filaInicio, columnaInicio + 2].Value = conteo.Lote_Contado;
        //                    excelWorksheet.Cells[filaInicio, columnaInicio + 3].Value = conteo.Usuario;
        //                    excelWorksheet.Cells[filaInicio, columnaInicio + 4].Value = conteo.Hora_Registro;
        //                }

        //                excelWorksheet.Cells[filaInicio, 22].Value = item.Stock_Final;
        //                excelWorksheet.Cells[filaInicio, 23].Value = item.Diferencia;
        //                excelWorksheet.Cells[filaInicio, 24].Value = item.Estado;


        //                filaInicio++;
        //            }

        //            // FOOTER TOTALES
        //            int filaTotal = 10 + reporte.TblReporteAuditoria.Count + 1;
        //            excelWorksheet.Cells[filaTotal, 2].Value = "Totales:";
        //            excelWorksheet.Cells[filaTotal, 2].Style.Font.Bold = true;

        //            excelWorksheet.Cells[filaTotal, 4].Value = reporte.Footer.StockInicialTotal; // Stock Inicial Total

        //            // Totales por conteo
        //            for (int i = 0; i < reporte.Footer.TotalesPorConteo.Count; i++)
        //            {
        //                var totalConteo = reporte.Footer.TotalesPorConteo[i];

        //                int columnaInicio = 7 + (i * 5);

        //                excelWorksheet.Cells[filaTotal, columnaInicio].Value = totalConteo.Total_Stock;
        //            }

        //            excelWorksheet.Cells[filaTotal, 22].Value = reporte.Footer.StockFinalTotal; // Stock Conteo Total
        //            excelWorksheet.Cells[filaTotal, 23].Value = reporte.Footer.StockDiferencial; // Diferencial Total

        //            // Formato para totales
        //            excelWorksheet.Cells[filaTotal, 4].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 12].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 17].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 22].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 23].Style.Font.Bold = true;
        //            excelWorksheet.Cells[filaTotal, 23].Style.Font.Color.SetColor(
        //                reporte.Footer.StockDiferencial < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
        //            );

        //            // Auto-ajustar columnas
        //            excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();
        //            for (int i = 1; i <= excelWorksheet.Dimension.End.Column; i++)
        //            {
        //                excelWorksheet.Column(i).Width += 3;
        //            }

        //            excelPackage.SaveAs(ms);
        //        }

        //        // IMPORTANTE
        //        ms.Position = 0;

        //        return new FileStreamResult(
        //            ms,
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //        )
        //        {
        //            FileDownloadName =
        //                $"Reporte_Auditoria_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Manejo del error
        //        return new HttpStatusCodeResult(
        //            500,
        //            ex.Message
        //        );
        //    }
        //}



        // EN DESUSO

        // GET: Vista Principal del Reporte(NO DIFERENCIAL,...
        [GenerateNonce]
        public ActionResult Reporte_Principal()
        {
            Session["NavIndex"] = "2";

            // Cargar combo de inventarios cerrados
            CombosBE objCombo = new CombosBE();
            objCombo.vchValue = "-1";  // Cambiado de intValue a vchValue
            objCombo.vchdesc = "Seleccionar Inventario";

            List<CombosBE> listInventariosCerrados = new List<CombosBE>();
            listInventariosCerrados = (List<CombosBE>)new CombosBL().cbxInventariosCerrados().Entity;
            listInventariosCerrados.Insert(0, objCombo);
            ViewBag.ListInventariosCerrados = listInventariosCerrados;

            return View();
        }

        //[GenerateNonce]
        //public ActionResult Reporte_Ubicacion()
        //{
        //    Session["NavIndex"] = "2";

        //    // Cargar combo de inventarios cerrados
        //    CombosBE objCombo = new CombosBE();
        //    objCombo.vchValue = "-1";  // Cambiado de intValue a vchValue
        //    objCombo.vchdesc = "Seleccionar Inventario";

        //    List<CombosBE> listInventariosCerrados = new List<CombosBE>();
        //    listInventariosCerrados = (List<CombosBE>)new CombosBL().cbxInventariosCerrados().Entity;
        //    listInventariosCerrados.Insert(0, objCombo);
        //    ViewBag.ListInventariosCerrados = listInventariosCerrados;

        //    return View();
        //}


        /// <summary>
        /// Método AJAX para recargar combo de inventarios cerrados
        /// </summary>
        [HttpPost]
        public JsonResult FillCbxInventariosCerrados()
        {
            try
            {
                CombosBE objCombo = new CombosBE();
                objCombo.vchValue = "-1";
                objCombo.vchdesc = "Seleccionar Inventario";

                List<CombosBE> listInventarios = new List<CombosBE>();
                listInventarios = (List<CombosBE>)new CombosBL().cbxInventariosCerrados().Entity;
                listInventarios.Insert(0, objCombo);

                return Json(new { success = true, data = listInventarios });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        #region Obtener Datos del Reporte

        /// <summary>
        /// Obtiene los datos principales del reporte de inventario cerrado
        /// Muestra: código producto, descripción, ubicación, lote, cantidad inicial, conteos y diferencias
        /// </summary>

        // NO EN DIFERENCIAL, ...
        [HttpPost]
        public JsonResult ObtenerDatosReporteInventario(
          string codInventario,
          int draw,
          int start,
          int length,
          string searchValue,
          int filtroDiferencias = 0)
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "asc" : "desc";
                int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
                string order = sortColumnIndexfix.ToString() + " " + sortDirection;

                var response = new ReportePrincipalBL().ObtenerReporteInventarioCerrado(
                    codInventario,
                    filtroDiferencias,
                    start.ToString(),
                    length.ToString(),
                    order,
                    searchValue ?? ""
                );

                // Validación robusta
                if (response == null || response.HUBO_ERROR)
                {
                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = 0,
                        recordsTotal = 0,
                        data = new List<object>(),
                        listFooter = new List<decimal>(),
                        infoInventario = new List<string>(),
                        error = response?.MENSAJE_ERROR ?? "Error al obtener datos"
                    });
                }

                // Convertir Entity directamente si es List
                List<TblReportePrincipalBE> lista;
                if (response.Entity is List<TblReportePrincipalBE>)
                {
                    lista = (List<TblReportePrincipalBE>)response.Entity;
                }
                else if (response.Entity is DataTable)
                {
                    lista = ConvertDataTableToList(response.Entity as DataTable);
                }
                else
                {
                    lista = new List<TblReportePrincipalBE>();
                }

                var totalData = response.count;
                List<decimal> listFooter = response.footerTable != null ?
                    (List<decimal>)response.footerTable : new List<decimal>();

                List<string> infoInventario = response.infoInventario != null ?
                    (List<string>)response.infoInventario : new List<string>();

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = totalData,
                    recordsTotal = totalData,
                    data = lista,
                    listFooter = listFooter,
                    infoInventario = infoInventario
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }

        // Método ReportePrincipalController
        // NO EN DIFERENCIAL, ...
        private List<TblReportePrincipalBE> ConvertDataTableToList(DataTable dt)
        {
            List<TblReportePrincipalBE> lista = new List<TblReportePrincipalBE>();

            if (dt == null || dt.Rows.Count == 0)
                return lista;

            foreach (DataRow row in dt.Rows)
            {
                TblReportePrincipalBE entity = new TblReportePrincipalBE();

                entity.Cod_Producto = row["COD_PRODUCTO"] != DBNull.Value ? row["COD_PRODUCTO"].ToString() : string.Empty;
                entity.Dsc_Producto = row["DSC_PRODUCTO"] != DBNull.Value ? row["DSC_PRODUCTO"].ToString() : string.Empty;
                entity.Cod_ubicacion = row["COD_UBICACION"] != DBNull.Value ? row["COD_UBICACION"].ToString() : string.Empty;
                entity.Lote_Producto = row["LOTE_PRODUCTO"] != DBNull.Value ? row["LOTE_PRODUCTO"].ToString() : string.Empty;

                entity.Stock_inicial = row["STOCK_INICIAL"] != DBNull.Value ? decimal.Parse(row["STOCK_INICIAL"].ToString()) : 0;
                entity.Conteo_1 = row["CONTEO_1"] != DBNull.Value ? decimal.Parse(row["CONTEO_1"].ToString()) : 0;
                entity.Conteo_2 = row["CONTEO_2"] != DBNull.Value ? decimal.Parse(row["CONTEO_2"].ToString()) : 0;
                entity.Conteo_3 = row["CONTEO_3"] != DBNull.Value ? decimal.Parse(row["CONTEO_3"].ToString()) : 0;
                entity.Stock_Final = row["STOCK_FINAL"] != DBNull.Value ? decimal.Parse(row["STOCK_FINAL"].ToString()) : 0;
                entity.Stock_Diferencial = row["DIFERENCIAL"] != DBNull.Value ? decimal.Parse(row["DIFERENCIAL"].ToString()) : 0;

                entity.Cod_Usuario_Registro = row["USUARIO_CONTEO"] != DBNull.Value ? row["USUARIO_CONTEO"].ToString() : string.Empty;
                entity.Fch_Registro_Inventario = row["FCH_REGISTRO_INVENTARIO"] != DBNull.Value ? row["FCH_REGISTRO_INVENTARIO"].ToString() : string.Empty;
                entity.Fch_Cierre = row["FCH_CIERRE"] != DBNull.Value ? row["FCH_CIERRE"].ToString() : string.Empty;
                entity.DSC_ANALISIS = row["DSC_ANALISIS"] != DBNull.Value ? row["DSC_ANALISIS"].ToString() : string.Empty;
                entity.DSC_OBSERVACION = row["DSC_OBSERVACION"] != DBNull.Value ? row["DSC_OBSERVACION"].ToString() : string.Empty;

                lista.Add(entity);
            }

            return lista;
        }



        [HttpGet]
        public ActionResult ExportarExcelInventarioCerrado(
        string codInventario,
        int filtroDiferencias = 0,
        string searchValue = "")
        {
            try
            {
                // Obtener TODOS los datos con los filtros aplicados
                var response = new ReportePrincipalBL().ObtenerReporteInventarioCerrado(
                    codInventario,
                    filtroDiferencias,
                    "0",      // start
                    "-1",     // length = -1 para traer todo
                    "1 ASC",  // order
                    searchValue ?? ""
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalBE> lista = (List<TblReportePrincipalBE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                // Convertir a DataTable
                DataTable dt = ConvertirListaADataTable(lista);

                // Crear Excel
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    // Título y datos del inventario
                    string titulo = "Reporte de Inventario Cerrado";
                    if (filtroDiferencias == 1) titulo += " - Con Diferencias";
                    if (filtroDiferencias == 2) titulo += " - Sin Diferencias";

                    excelWorksheet.Cells["D3"].Value = titulo;
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : ""; // Nombre Almacén
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    // Cabeceras en fila 11
                    string[] headers = new string[] {
                    "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                    "STOCK INICIAL", "CONTEO 1", "CONTEO 2", "CONTEO 3",
                    "STOCK FINAL", "DIFERENCIAL", "USUARIO"
                };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                    }

                    // Datos desde fila 12
                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    // Totales
                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    if (listFooter.Count >= 6)
                    {
                        excelWorksheet.Cells[filaTotal, 6].Value = listFooter[0]; // Stock Inicial
                        excelWorksheet.Cells[filaTotal, 7].Value = listFooter[1]; // Conteo 1
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2]; // Conteo 2
                        excelWorksheet.Cells[filaTotal, 9].Value = listFooter[3]; // Conteo 3
                        excelWorksheet.Cells[filaTotal, 10].Value = listFooter[4]; // Stock Final
                        excelWorksheet.Cells[filaTotal, 11].Value = listFooter[5]; // Diferencial
                    }

                    // Logo
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Inventario_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        private DataTable ConvertirListaADataTable(List<TblReportePrincipalBE> lista)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("COD_PRODUCTO", typeof(string));
            dt.Columns.Add("DSC_PRODUCTO", typeof(string));
            dt.Columns.Add("COD_UBICACION", typeof(string));
            dt.Columns.Add("LOTE_PRODUCTO", typeof(string));
            dt.Columns.Add("STOCK_INICIAL", typeof(decimal));
            dt.Columns.Add("CONTEO_1", typeof(decimal));
            dt.Columns.Add("CONTEO_2", typeof(decimal));
            dt.Columns.Add("CONTEO_3", typeof(decimal));
            dt.Columns.Add("STOCK_FINAL", typeof(decimal));
            dt.Columns.Add("DIFERENCIAL", typeof(decimal));
            dt.Columns.Add("USUARIO", typeof(string));

            foreach (var item in lista)
            {
                dt.Rows.Add(
                    item.Cod_Producto ?? "",
                    item.Dsc_Producto ?? "",
                    item.Cod_ubicacion ?? "",
                    item.Lote_Producto ?? "",
                    item.Stock_inicial,
                    item.Conteo_1,
                    item.Conteo_2,
                    item.Conteo_3,
                    item.Stock_Final,
                    item.Stock_Diferencial,
                    item.Cod_Usuario_Registro ?? ""
                );
            }

            return dt;
        }


        // ===================================================
        // MÉTODOS NUEVOS PARA LOS 4 TIPOS DE REPORTES V2
        // ===================================================

        #region Reportes V2 - Último Conteo Finalizado

        /// <summary>
        /// Obtiene datos del reporte DIFERENCIAL (solo productos con diferencias)
        /// </summary>
        [HttpPost]
        public JsonResult ObtenerDatosReporteDiferencial(
            string codInventario,
            int draw,
            int start,
            int length,
            string searchValue = "")
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "asc" : "desc";
                int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
                string order = sortColumnIndexfix.ToString() + " " + sortDirection;

                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "DIFERENCIAL",
                    searchValue ?? "",
                    start.ToString(),
                    length.ToString(),
                    order
                );

                return ProcessReporteV2Response(response, draw);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene datos del reporte por PRODUCTO
        /// </summary>
        [HttpPost]
        public JsonResult ObtenerDatosReporteProducto(
            string codInventario,
            string busqueda,
            int draw,
            int start,
            int length)
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "asc" : "desc";
                int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
                string order = sortColumnIndexfix.ToString() + " " + sortDirection;

                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "PRODUCTO",
                    busqueda ?? "",
                    start.ToString(),
                    length.ToString(),
                    order
                );

                return ProcessReporteV2Response(response, draw);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene datos del reporte por UBICACIÓN
        /// </summary>
        [HttpPost]
        public JsonResult ObtenerDatosReporteUbicacion(
            string codInventario,
            string busqueda,
            int draw,
            int start,
            int length)
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "asc" : "desc";
                int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
                string order = sortColumnIndexfix.ToString() + " " + sortDirection;

                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "UBICACION",
                    busqueda ?? "",
                    start.ToString(),
                    length.ToString(),
                    order
                );

                return ProcessReporteV2Response(response, draw);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene datos del reporte por USUARIO
        /// </summary>
        [HttpPost]
        public JsonResult ObtenerDatosReporteUsuario(
            string codInventario,
            string busqueda,
            int draw,
            int start,
            int length)
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "asc" : "desc";
                int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
                string order = sortColumnIndexfix.ToString() + " " + sortDirection;

                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "USUARIO",
                    busqueda ?? "",
                    start.ToString(),
                    length.ToString(),
                    order
                );

                return ProcessReporteV2Response(response, draw);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }



        #region Exportar Excel V2 - Reportes con Último Conteo Finalizado

        /// <summary>
        /// Exporta a Excel el reporte DIFERENCIAL
        /// </summary>
        [HttpGet]
        public ActionResult ExportarExcelReporteDiferencial(string codInventario, string searchValue = "")
        {
            try
            {
                // Obtener TODOS los datos
                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "DIFERENCIAL",
                    searchValue ?? "",
                    "0",
                    "-1",
                    null
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                // Convertir a DataTable
                DataTable dt = ConvertirListaV2ADataTable(lista);

                // Crear Excel
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    // Título
                    excelWorksheet.Cells["D3"].Value = "Reporte de Diferencial - Último Conteo Finalizado";
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : ""; // Nombre Almacén
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    // Info adicional
                    if (infoInventario.Count > 5)
                    {
                        excelWorksheet.Cells["E9"].Value = $"Último Conteo Cerrado: {infoInventario[5]}";
                        excelWorksheet.Cells["G9"].Value = $"Estado del Inventario: {infoInventario[4]}";
                    }

                    // Cabeceras en fila 11
                    string[] headers = new string[] {
                        "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                        "STOCK INICIAL", "ÚLTIMO CONTEO", "STOCK CONTEO", "DIFERENCIAL", "USUARIO"
                    };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                    }

                    // Datos desde fila 12
                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    // Totales
                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    excelWorksheet.Cells[filaTotal, 1].Style.Font.Bold = true;

                    if (listFooter.Count >= 3)
                    {
                        excelWorksheet.Cells[filaTotal, 5].Value = listFooter[0]; // Stock Inicial
                        excelWorksheet.Cells[filaTotal, 7].Value = listFooter[1]; // Stock Último Conteo
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2]; // Diferencial

                        // Formato para totales
                        excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                            listFooter[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                        );
                    }

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    // Logo
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Reporte_Diferencial_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Reporte_Diferencial");
            }
        }

        /// <summary>
        /// Exporta a Excel el reporte por PRODUCTO
        /// </summary>
        [HttpGet]
        public ActionResult ExportarExcelReporteProducto(string codInventario, string busqueda = "")
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "PRODUCTO",
                    busqueda ?? "",
                    "0",
                    "-1",
                    null
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                DataTable dt = ConvertirListaV2ADataTable(lista);
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    string titulo = "Reporte por Producto - Último Conteo Finalizado";
                    if (!string.IsNullOrEmpty(busqueda))
                        titulo += $" - Búsqueda: {busqueda}";

                    excelWorksheet.Cells["D3"].Value = titulo;
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : "";
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    if (infoInventario.Count > 5)
                    {
                        excelWorksheet.Cells["E9"].Value = $"Último Conteo: {infoInventario[5]}";
                        excelWorksheet.Cells["G9"].Value = $"Estado: {infoInventario[4]}";
                    }

                    string[] headers = new string[] {
                "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                "STOCK INICIAL", "ÚLTIMO CONTEO", "STOCK CONTEO", "DIFERENCIAL", "USUARIO"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                    }

                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    excelWorksheet.Cells[filaTotal, 1].Style.Font.Bold = true;

                    if (listFooter.Count >= 3)
                    {
                        excelWorksheet.Cells[filaTotal, 5].Value = listFooter[0];
                        excelWorksheet.Cells[filaTotal, 7].Value = listFooter[1];
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2];

                        // Formato para totales
                        excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                            listFooter[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                        );
                    }

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Reporte_Producto_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Reporte_Producto");
            }
        }

        /// <summary>
        /// Exporta a Excel el reporte por UBICACIÓN
        /// </summary>
        [HttpGet]
        public ActionResult ExportarExcelReporteUbicacion(string codInventario, string busqueda = "")
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "UBICACION",
                    busqueda ?? "",
                    "0",
                    "-1",
                    null
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                DataTable dt = ConvertirListaV2ADataTable(lista);
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    string titulo = "Reporte por Ubicación - Último Conteo Finalizado";
                    if (!string.IsNullOrEmpty(busqueda))
                        titulo += $" - Búsqueda: {busqueda}";

                    excelWorksheet.Cells["D3"].Value = titulo;
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : "";
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    if (infoInventario.Count > 5)
                    {
                        excelWorksheet.Cells["E9"].Value = $"Último Conteo Cerrado: {infoInventario[5]}";
                        excelWorksheet.Cells["G9"].Value = $"Estado del inventario: {infoInventario[4]}";
                    }

                    string[] headers = new string[] {
                "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                "STOCK INICIAL", "ÚLTIMO CONTEO", "STOCK CONTEO", "DIFERENCIAL", "USUARIO"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                    }

                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    excelWorksheet.Cells[filaTotal, 1].Style.Font.Bold = true;

                    if (listFooter.Count >= 3)
                    {
                        excelWorksheet.Cells[filaTotal, 5].Value = listFooter[0];
                        excelWorksheet.Cells[filaTotal, 7].Value = listFooter[1];
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2];

                        // Formato para totales
                        excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                            listFooter[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                        );

                    }

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Reporte_Ubicacion_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Reporte_Ubicacion");
            }
        }

        /// <summary>
        /// Exporta a Excel el reporte por USUARIO
        /// </summary>
        [HttpGet]
        public ActionResult ExportarExcelReporteUsuario(string codInventario, string busqueda = "")
        {
            try
            {
                var response = new ReportePrincipalBL().ObtenerReporteInventarioV2(
                    codInventario,
                    "USUARIO",
                    busqueda ?? "",
                    "0",
                    "-1",
                    null
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                DataTable dt = ConvertirListaV2ADataTable(lista);
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    string titulo = "Reporte por Usuario - Último Conteo Finalizado";
                    if (!string.IsNullOrEmpty(busqueda))
                        titulo += $" - Búsqueda: {busqueda}";

                    excelWorksheet.Cells["D3"].Value = titulo;
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : "";
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    if (infoInventario.Count > 5)
                    {
                        excelWorksheet.Cells["E9"].Value = $"Último Conteo Cerrado: {infoInventario[5]}";
                        excelWorksheet.Cells["G9"].Value = $"Estado del invenatrio: {infoInventario[4]}";
                    }

                    string[] headers = new string[] {
                "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                "STOCK INICIAL", "ÚLTIMO CONTEO", "STOCK CONTEO", "DIFERENCIAL", "USUARIO"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                    }

                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    excelWorksheet.Cells[filaTotal, 1].Style.Font.Bold = true;

                    if (listFooter.Count >= 3)
                    {
                        excelWorksheet.Cells[filaTotal, 5].Value = listFooter[0];
                        excelWorksheet.Cells[filaTotal, 6].Value = listFooter[1];
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2];

                        // Formato para totales
                        excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                            listFooter[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                        );
                    }

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Reporte_Usuario_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Reporte_Usuario");
            }
        }

        #endregion

        #region Métodos auxiliares para Reportes V2

        /// <summary>
        /// Convierte lista V2 a DataTable para Excel
        /// </summary>
        private DataTable ConvertirListaV2ADataTable(List<TblReportePrincipalV2BE> lista)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("COD_PRODUCTO", typeof(string));
            dt.Columns.Add("DSC_PRODUCTO", typeof(string));
            dt.Columns.Add("COD_UBICACION", typeof(string));
            dt.Columns.Add("LOTE_PRODUCTO", typeof(string));
            //dt.Columns.Add("SERIE_PRODUCTO", typeof(string));
            dt.Columns.Add("STOCK_INICIAL", typeof(decimal));
            dt.Columns.Add("ULTIMO_CONTEO", typeof(int));
            dt.Columns.Add("STOCK_ULTIMO_CONTEO", typeof(decimal));
            dt.Columns.Add("DIFERENCIAL", typeof(decimal));
            dt.Columns.Add("USUARIO", typeof(string));

            foreach (var item in lista)
            {
                dt.Rows.Add(
                    item.Cod_Producto ?? "",
                    item.Dsc_Producto ?? "",
                    item.Cod_ubicacion ?? "",
                    item.Lote_Producto ?? "",
                    //item.Serie_Producto ?? "",
                    item.Stock_inicial,
                    item.Ultimo_Conteo_Finalizado,
                    item.Stock_Ultimo_Conteo,
                    item.Stock_Diferencial,
                    item.Cod_Usuario_Registro ?? ""
                );
            }

            return dt;
        }

        /// <summary>
        /// Método auxiliar para procesar respuestas de reportes V2
        /// </summary>
        private JsonResult ProcessReporteV2Response(Response response, int draw)
        {
            if (response == null || response.HUBO_ERROR)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = response?.MENSAJE_ERROR ?? "Error al obtener datos"
                });
            }

            List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
            var totalData = response.count;
            List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
            List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

            return Json(new
            {
                draw = draw,
                recordsFiltered = totalData,
                recordsTotal = totalData,
                data = lista,
                listFooter = listFooter,
                infoInventario = infoInventario
            });
        }

        #endregion

        #endregion
        #endregion


        //Reportes por conteo
        // ============================================
        // MÉTODO PARA DATATABLES (AJAX)
        // ============================================
        [HttpPost]
        public JsonResult ObtenerDatosReportePorConteo(
            string codInventario,
            int nroConteo,
            int draw,
            int start,
            int length)
        {
            try
            {
                string sortColumnIndex = Request.Form["order[0][column]"]?.ToString() ?? "0";
                string sortColumnDirection = Request.Form["order[0][dir]"]?.ToString() ?? "asc";
                var sortDirection = sortColumnDirection == "asc" ? "ASC" : "DESC";

                // Mapeo de columnas para ordenamiento
                string[] columnas = new string[] {
            "COD_PRODUCTO",
            "DSC_PRODUCTO",
            "COD_UBICACION",
            "LOTE_PRODUCTO",
            "SERIE_PRODUCTO",
            "STOCK_INICIAL",
            "STOCK_CONTEO",
            "DIFERENCIAL",
            "COD_USUARIO_REGISTRO"
        };

                int columnIndex = Int32.Parse(sortColumnIndex);
                string sortColumn = columnIndex < columnas.Length ? columnas[columnIndex] : "COD_PRODUCTO";
                string order = $"{sortColumn} {sortDirection}";

                var response = new ReportePrincipalBL().ObtenerReporteInventarioPorConteo(
                    codInventario,
                    nroConteo,
                    start.ToString(),
                    length.ToString(),
                    order
                );

                if (response.HUBO_ERROR)
                {
                    return Json(new
                    {
                        draw = draw,
                        recordsFiltered = 0,
                        recordsTotal = 0,
                        data = new List<object>(),
                        listFooter = new List<decimal>(),
                        infoInventario = new List<string>(),
                        error = response.MENSAJE_ERROR
                    });
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = response.count,
                    recordsTotal = response.count,
                    data = lista,
                    listFooter = listFooter,
                    infoInventario = infoInventario
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = draw,
                    recordsFiltered = 0,
                    recordsTotal = 0,
                    data = new List<object>(),
                    listFooter = new List<decimal>(),
                    infoInventario = new List<string>(),
                    error = ex.Message
                });
            }
        }

        // ============================================
        // MÉTODO PARA EXPORTAR A EXCEL POR CONTEO
        // ============================================
        [HttpGet]
        public ActionResult ExportarExcelReportePorConteo(string codInventario, int nroConteo)
        {
            try
            {
                // Obtener TODOS los datos del conteo específico
                var response = new ReportePrincipalBL().ObtenerReporteInventarioPorConteo(
                    codInventario,
                    nroConteo,
                    "0",
                    "-1", // Sin límite
                    "COD_PRODUCTO ASC"
                );

                if (response == null || response.HUBO_ERROR || response.Entity == null)
                {
                    throw new Exception(response?.MENSAJE_ERROR ?? "Error al obtener datos");
                }

                List<TblReportePrincipalV2BE> lista = (List<TblReportePrincipalV2BE>)response.Entity;
                List<decimal> listFooter = response.footerTable != null ? (List<decimal>)response.footerTable : new List<decimal>();
                List<string> infoInventario = response.infoInventario != null ? (List<string>)response.infoInventario : new List<string>();

                // REUTILIZAR el método existente
                DataTable dt = ConvertirListaV2ADataTable(lista);

                // Crear Excel
                MemoryStream ms = new MemoryStream();
                string plantilla = Server.MapPath(@"~\Plantillas\Plantilla_Reportes.xlsx");

                using (FileStream fs = System.IO.File.OpenRead(plantilla))
                using (ExcelPackage excelPackage = new ExcelPackage(fs))
                {
                    ExcelWorkbook excelWorkBook = excelPackage.Workbook;
                    ExcelWorksheet excelWorksheet = excelWorkBook.Worksheets[1];

                    // Título
                    excelWorksheet.Cells["D3"].Value = $"Reporte de Inventario - Conteo #{nroConteo}";
                    excelWorksheet.Cells["E8"].Value = infoInventario.Count > 0 ? infoInventario[0] : ""; // Nombre Almacén
                    excelWorksheet.Cells["G8"].Value = codInventario;

                    // Info adicional del conteo
                    if (infoInventario.Count >= 6)
                    {
                        excelWorksheet.Cells["E9"].Value = $"Número de Conteo: {infoInventario[4]}"; // Nro Conteo
                        excelWorksheet.Cells["G9"].Value = $"Estado: {infoInventario[5]}"; // Estado Conteo
                    }

                    if (infoInventario.Count >= 3)
                    {
                        excelWorksheet.Cells["E10"].Value = $"Fecha Registro: {infoInventario[1]}";
                        if (!string.IsNullOrEmpty(infoInventario[2]))
                        {
                            excelWorksheet.Cells["G10"].Value = $"Fecha Cierre: {infoInventario[2]}";
                        }
                    }

                    // Cabeceras en fila 11
                    string[] headers = new string[] {
                "CÓDIGO", "PRODUCTO", "UBICACIÓN", "LOTE",
                "STOCK INICIAL", "CONTEO #", "STOCK CONTEO", "DIFERENCIAL", "USUARIO"
            };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        excelWorksheet.Cells[11, i + 1].Value = headers[i];
                        //excelWorksheet.Cells[11, i + 1].Style.Font.Bold = true;
                        //excelWorksheet.Cells[11, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //excelWorksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Datos desde fila 12
                    if (dt.Rows.Count > 0)
                    {
                        excelWorksheet.Cells["A12"].LoadFromDataTable(dt, false);
                    }

                    // Totales
                    int filaTotal = 12 + dt.Rows.Count + 1;
                    excelWorksheet.Cells[filaTotal, 1].Value = "TOTALES:";
                    excelWorksheet.Cells[filaTotal, 1].Style.Font.Bold = true;

                    if (listFooter.Count >= 3)
                    {
                        excelWorksheet.Cells[filaTotal, 5].Value = listFooter[0]; // Stock Inicial
                        excelWorksheet.Cells[filaTotal, 7].Value = listFooter[1]; // Stock Conteo
                        excelWorksheet.Cells[filaTotal, 8].Value = listFooter[2]; // Diferencial

                        // Formato para totales
                        excelWorksheet.Cells[filaTotal, 5].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 7].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Bold = true;
                        excelWorksheet.Cells[filaTotal, 8].Style.Font.Color.SetColor(
                            listFooter[2] < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Green
                        );
                    }

                    // Estadísticas adicionales
                    if (listFooter.Count >= 5)
                    {
                        int filaStats = filaTotal + 2;
                        excelWorksheet.Cells[filaStats, 1].Value = "ESTADÍSTICAS:";
                        excelWorksheet.Cells[filaStats, 1].Style.Font.Bold = true;

                        excelWorksheet.Cells[filaStats + 1, 1].Value = "Productos con Diferencias:";
                        excelWorksheet.Cells[filaStats + 1, 2].Value = listFooter[3];

                        excelWorksheet.Cells[filaStats + 2, 1].Value = "Productos Exactos:";
                        excelWorksheet.Cells[filaStats + 2, 2].Value = listFooter[4];
                    }

                    // Auto-ajustar columnas
                    excelWorksheet.Cells[excelWorksheet.Dimension.Address].AutoFitColumns();

                    // Logo
                    string rutaImagen = Server.MapPath(@"~\Assets\IMG\LogoExcel.png");
                    if (System.IO.File.Exists(rutaImagen))
                    {
                        using (System.Drawing.Image imagen = System.Drawing.Image.FromFile(rutaImagen))
                        {
                            var picture = excelWorksheet.Drawings.AddPicture("Imagen", imagen);
                            picture.SetPosition(0, 0);
                            picture.SetSize(260, 111);
                        }
                    }

                    excelPackage.SaveAs(ms);
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = $"Reporte_Conteo_{nroConteo}_{codInventario}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index"); // Cambia por tu action principal
            }
        }
    }
}