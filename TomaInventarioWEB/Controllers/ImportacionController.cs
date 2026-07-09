using BE;
using BL;
using ExcelDataReader;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace TomaInventarioWEB.Controllers
{
    [CheckSession]
    public class ImportacionController : Controller
    {
        // GET: Importacion
        [GenerateNonce]
        public ActionResult ImportacionArchivos()
        {
            Session["NavIndex"] = "2";

            return View();
        }

        public ActionResult DescargarExcelMaestro()
        {
            // Ruta del archivo en el servidor
            string rutaArchivo = Server.MapPath("~/Plantillas/1.CARGA - MAESTROS.xlsx");

            // Nombre que se mostrará al descargar el archivo
            string nombreArchivo = "CARGA - MAESTROS.xlsx";

            // Tipo MIME para un archivo Excel
            string tipoMIME = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            return File(rutaArchivo, tipoMIME, nombreArchivo);
        }

        public ActionResult DescargarExcelInventario()
        {
            // Ruta del archivo en el servidor
            string rutaArchivo = Server.MapPath("~/Plantillas/2.CARGA - INVENTARIO.xlsx");

            // Nombre que se mostrará al descargar el archivo
            string nombreArchivo = "CARGA - INVENTARIO.xlsx";

            // Tipo MIME para un archivo Excel
            string tipoMIME = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            return File(rutaArchivo, tipoMIME, nombreArchivo);
        }

        public ActionResult DescargarExcelProductos()
        {
            // Ruta del archivo en el servidor
            string rutaArchivo = Server.MapPath("~/Plantillas/Plantilla_Producto.xlsx");

            // Nombre que se mostrará al descargar el archivo
            string nombreArchivo = "Plantilla_Producto.xlsx";

            // Tipo MIME para un archivo Excel
            string tipoMIME = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            // Obtener la lista de unidades de medida desde la base de datos
            List<UMEXCELBE> listaUnidades = new MantenimientosBL().ListarUnidadMedidaEXCEL();

            // Crear un nuevo paquete de Excel a partir del archivo existente
            using (ExcelPackage excelPackage = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                // Obtener la Hoja1
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Hoja1"];

                if (worksheet != null && listaUnidades.Any())
                {
                    // Determinar la fila y columna de inicio para la tabla
                    int startRow = 5;
                    int startColumn = 6; // Corresponde a la columna F

                    // Obtener las propiedades de la clase UnidadMedidaBE para los encabezados
                    var properties = typeof(UMEXCELBE).GetProperties();

                    // Insertar los datos de la lista
                    for (int i = 0; i < listaUnidades.Count; i++)
                    {
                        for (int j = 0; j < properties.Length; j++)
                        {
                            var propertyValue = properties[j].GetValue(listaUnidades[i]);
                            worksheet.Cells[startRow + i, startColumn + j].Value = propertyValue;
                        }
                    }

                    // Determinar el rango de la tabla (incluyendo encabezados)
                    int endRow = startRow + listaUnidades.Count - 1;
                    int endColumn = startColumn + properties.Length - 1;
                    ExcelRange dataRange = worksheet.Cells[5, 6, endRow, endColumn];

                    // Opcional: Formatear como tabla
                    ExcelTable table = worksheet.Tables.Add(dataRange, "TablaUnidadesMedida");
                    table.ShowHeader = false;
                    table.TableStyle = TableStyles.Light1;

                    // Ajustar el ancho de las columnas
                    //worksheet.Cells[dataRange.Address].AutoFitColumns();

                    // Crear un MemoryStream para guardar el archivo modificado
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        excelPackage.SaveAs(memoryStream);
                        memoryStream.Position = 0;

                        return File(memoryStream.ToArray(), tipoMIME, nombreArchivo);
                    }
                }
                else
                {
                    return Content("Error: No se encontró la Hoja1 en la plantilla o no hay datos de unidades de medida.");
                }
            }



            //return File(rutaArchivo, tipoMIME, nombreArchivo);
        }

        public ActionResult DescargarExcelUbicaciones()
        {
            // Ruta del archivo en el servidor
            string rutaArchivo = Server.MapPath("~/Plantillas/Plantilla_Ubicacion.xlsx");

            // Nombre que se mostrará al descargar el archivo
            string nombreArchivo = "Plantilla_Ubicaciones.xlsx";

            // Tipo MIME para un archivo Excel
            string tipoMIME = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            return File(rutaArchivo, tipoMIME, nombreArchivo);
        }


        public ActionResult DescargarExcelDetalle()
        {
            // Ruta del archivo en el servidor
            string rutaArchivo = Server.MapPath("~/Plantillas/Plantilla_Detalle.xlsx");

            // Nombre que se mostrará al descargar el archivo
            string nombreArchivo = "Plantilla_Detalle.xlsx";

            // Tipo MIME para un archivo Excel
            string tipoMIME = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


            return File(rutaArchivo, tipoMIME, nombreArchivo);
        }

        #region Importacion Detalle
        public JsonResult SubirArchivo_Detalle(HttpPostedFileBase archivo, int IdAlmacen)
        {
            //SE CREA IMPORTACIONID Y SE ENVIA
            Guid importacionId = Guid.NewGuid();

            Response response = new BE.Response();
            List<DetInventarioImportBE> ListaResult = new List<DetInventarioImportBE>();

            if (archivo == null || archivo.ContentLength <= 0)
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se seleccionó ningún archivo o el archivo está vacío.";
                return Json(response);
            }

            string extension = Path.GetExtension(archivo.FileName).ToLower();

            if (extension != ".xls" && extension != ".xlsx")
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "La extensión del archivo no es valida.";
                return Json(response);
            }

            using (var stream = archivo.InputStream)
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    });

                    // Obtener la tabla del DataSet (asumiendo que es la primera)
                    DataTable dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count == 0)
                    {
                        response.HUBO_ERROR = true;
                        response.MENSAJE_ERROR = "El archivo no contiene registros.";
                        return Json(response);
                    }

                    dataTable.TableName = "Table1";

                    string xmlData = ConvertDataTableToXml(dataTable);

                    var dataAccess = new InventarioBL();

                    ListaResult = dataAccess.ImportarDetalles(
                         xmlData,
                         IdAlmacen,
                         Session["UserName"].ToString(),
                         importacionId
                    );

                    //0: error, 1: correcto
                    // TODO ENVIAR AL RESPONSE Y IMPRIMIR EN FRONT

                    ResultadoImportacionDetInventarioImportBE resultado = new ResultadoImportacionDetInventarioImportBE
                    {
                        ListaCorrectos = ListaResult.Where(x => x.Flg_Pass == 1).ToList(),
                        ListaIncorrectos = ListaResult.Where(x => x.Flg_Pass == 0).ToList(),
                        Errores = ListaResult.Count(x => x.Flg_Pass == 0),
                        Correctos = ListaResult.Count(x => x.Flg_Pass == 1),
                        TotalImportacion = ListaResult.Count()
                    };

                    response.Entity = resultado;

                    // TODO ENVIAR AL RESPONSE Y IMPRIMIR EN FRONT
                    //int errores = ListaResult.Count(x => x.Flg_Pass == 0);
                    //int correctos = ListaResult.Count(x => x.Flg_Pass == 1);
                    //int totales = errores + correctos;
                    //response.Entity = ListaResult;
                    //response.Entity = ListaResult.Where(x => x.Flg_Pass == 0);
                }
            }
            //return Json(response);
            // SOLUCION TEMPORAL - CAMBIAR EL FLUJO
            return new JsonResult
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }


        #endregion
        #region Importacion de Mantenimientos
        public DataTable FiltrarTablaProductos(DataTable originalTable)
        {
            DataTable filteredTable = new DataTable();

            if (originalTable != null)
            {
                // Definir las columnas que queremos en la nueva tabla
                if (originalTable.Columns.Contains("COD_PRODUCTO"))
                {
                    filteredTable.Columns.Add("COD_PRODUCTO", originalTable.Columns["COD_PRODUCTO"].DataType);
                }
                if (originalTable.Columns.Contains("DSC_PRODUCTO"))
                {
                    filteredTable.Columns.Add("DSC_PRODUCTO", originalTable.Columns["DSC_PRODUCTO"].DataType);
                }
                if (originalTable.Columns.Contains("COD_UNIDAD_MEDIDA"))
                {
                    filteredTable.Columns.Add("COD_UNIDAD_MEDIDA", originalTable.Columns["COD_UNIDAD_MEDIDA"].DataType);
                }

                // Filtrar las filas y copiar los datos
                foreach (DataRow row in originalTable.Rows)
                {
                    bool hasContent = false;
                    DataRow newRow = filteredTable.NewRow();

                    if (filteredTable.Columns.Contains("COD_PRODUCTO") && originalTable.Columns.Contains("COD_PRODUCTO") && !string.IsNullOrWhiteSpace(row["COD_PRODUCTO"]?.ToString()))
                    {
                        newRow["COD_PRODUCTO"] = row["COD_PRODUCTO"];
                        hasContent = true;
                    }
                    if (filteredTable.Columns.Contains("DSC_PRODUCTO") && originalTable.Columns.Contains("DSC_PRODUCTO") && !string.IsNullOrWhiteSpace(row["DSC_PRODUCTO"]?.ToString()))
                    {
                        newRow["DSC_PRODUCTO"] = row["DSC_PRODUCTO"];
                        hasContent = true;
                    }
                    if (filteredTable.Columns.Contains("COD_UNIDAD_MEDIDA") && originalTable.Columns.Contains("COD_UNIDAD_MEDIDA") && !string.IsNullOrWhiteSpace(row["COD_UNIDAD_MEDIDA"]?.ToString()))
                    {
                        newRow["COD_UNIDAD_MEDIDA"] = row["COD_UNIDAD_MEDIDA"];
                        hasContent = true;
                    }

                    if (hasContent)
                    {
                        filteredTable.Rows.Add(newRow);
                    }
                }
            }

            return filteredTable;
        }

        public JsonResult SubirArchivo_Producto(HttpPostedFileBase archivo)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();
            if (archivo != null && archivo.ContentLength > 0)
            {
                string[] filename = archivo.FileName.Split('.');
                string extension = filename[1];
                if (extension != "xls" && extension != "xlsx")
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "La extensión del archivo no es valida.";
                    return Json(response);
                }

                using (var stream = archivo.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        // Obtener la tabla del DataSet (asumiendo que es la primera)
                        DataTable dataTable = dataSet.Tables[0];
                        dataTable = FiltrarTablaProductos(dataTable);

                        // Calcular el número total de registros
                        int totalRecords = dataTable.Rows.Count;

                        if (totalRecords > 0)
                        {
                            // Determinar si es necesario dividir el XML en bloques
                            bool splitXml = totalRecords > 100;

                            // Si no es necesario dividir el XML, enviarlo directamente
                            dataTable.TableName = "Table1";
                            if (!splitXml)
                            {
                                string xmlData = ConvertDataTableToXml(dataTable);
                                var dataAccess = new MantenimientosBL();
                                ListaResult = dataAccess.ImportarProductos(xmlData);
                            }
                            else
                            {
                                // Dividir el XML en bloques de 100 registros
                                int startIndex = 0;
                                int blockSize = 500;
                                var dataAccess = new MantenimientosBL();

                                while (startIndex < totalRecords)
                                {
                                    // Obtener el bloque actual de registros
                                    var blockRows = dataTable.AsEnumerable()
                                        .Skip(startIndex)
                                        .Take(blockSize)
                                        .CopyToDataTable();


                                    // Convertir el bloque de registros a XML
                                    string xmlDataBlock = ConvertDataTableToXml(blockRows);

                                    // Enviar el bloque de registros a ImportarEmpleados

                                    ListaResult.AddRange(dataAccess.ImportarProductos(xmlDataBlock));
                                    //if (startIndex == 0)
                                    //{
                                    //    ListaResult =  dataAccess.ImportarProductos(xmlDataBlock);
                                    //}
                                    //else
                                    //{
                                    //    ListaResult.AddRange(dataAccess.ImportarProductos(xmlDataBlock));
                                    //}

                                    // Incrementar el índice para el siguiente bloque
                                    startIndex += blockSize;
                                }

                            }
                        }
                        //response.Entity = ListaResult;
                        //response.Entity = ListaResult.Where(x => x.Flg_pass == 0).ToList();
                        //0: error, 1: actualizado, 2: nuevos, 3:sin cambios
                        // TODO ENVIAR AL RESPONSE Y IMPRIMIR EN FRONT

                        ResultadoImportacionBE resultado = new ResultadoImportacionBE
                        {
                            Lista = ListaResult.Where(x => x.Flg_pass == 0 || x.Flg_pass == 1).ToList(),
                            Errores = ListaResult.Count(x => x.Flg_pass == 0),
                            Actualizados = ListaResult.Count(x => x.Flg_pass == 1),
                            Nuevos = ListaResult.Count(x => x.Flg_pass == 2),
                            SinCambios = ListaResult.Count(x => x.Flg_pass == 3),
                        };

                        response.Entity = resultado;
                    }
                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se seleccionó ningún archivo o el archivo está vacío.";
                return Json(response);
            }
            //response.Entity = null;

            //return Json(response);
            return new JsonResult
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public JsonResult SubirArchivo_Ubicaciones(HttpPostedFileBase archivo)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();
            if (archivo != null && archivo.ContentLength > 0)
            {
                string[] filename = archivo.FileName.Split('.');
                string extension = filename[1];
                if (extension != "xls" && extension != "xlsx")
                {
                    response.HUBO_ERROR = true;
                    response.MENSAJE_ERROR = "La extensión del archivo no es valida.";
                    return Json(response);
                }

                using (var stream = archivo.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });

                        // Obtener la tabla del DataSet (asumiendo que es la primera)
                        DataTable dataTable = dataSet.Tables[0];


                        // Calcular el número total de registros ConvertDataTable
                        int totalRecords = dataTable.Rows.Count;

                        if (totalRecords > 0)
                        {
                            // Determinar si es necesario dividir el XML en bloques
                            bool splitXml = totalRecords > 100;

                            // Si no es necesario dividir el XML, enviarlo directamente
                            dataTable.TableName = "Table1";
                            if (!splitXml)
                            {
                                string xmlData = ConvertDataTableToXml(dataTable);
                                var dataAccess = new MantenimientosBL();
                                ListaResult = dataAccess.ImportarUbicaciones(xmlData);
                            }
                            else
                            {
                                // Dividir el XML en bloques de 100 registros
                                int startIndex = 0;
                                int blockSize = 500;
                                var dataAccess = new MantenimientosBL();

                                while (startIndex < totalRecords)
                                {
                                    // Obtener el bloque actual de registros
                                    var blockRows = dataTable.AsEnumerable()
                                        .Skip(startIndex)
                                        .Take(blockSize)
                                        .CopyToDataTable();


                                    // Convertir el bloque de registros a XML
                                    string xmlDataBlock = ConvertDataTableToXml(blockRows);

                                    // Enviar el bloque de registros a ImportarEmpleados

                                    ListaResult.AddRange(dataAccess.ImportarUbicaciones(xmlDataBlock));

                                    // Incrementar el índice para el siguiente bloque
                                    startIndex += blockSize;
                                }

                            }
                        }
                        //response.Entity = ListaResult;
                        //0: error, 1: actualizado, 2: nuevos, 3:sin cambios
                        // TODO ENVIAR AL RESPONSE Y IMPRIMIR EN FRONT

                        ResultadoImportacionBE resultado = new ResultadoImportacionBE
                        {
                            Lista = ListaResult.Where(x => x.Flg_pass == 0 || x.Flg_pass == 1).ToList(),
                            Errores = ListaResult.Count(x => x.Flg_pass == 0),
                            Actualizados = ListaResult.Count(x => x.Flg_pass == 1),
                            Nuevos = ListaResult.Count(x => x.Flg_pass == 2),
                            SinCambios = ListaResult.Count(x => x.Flg_pass == 3),
                        };

                        response.Entity = resultado;
                    }
                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se seleccionó ningún archivo o el archivo está vacío.";
                return Json(response);
            }

            //return Json(response);
            return new JsonResult
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        #endregion

        #region Importacion de Archivos Maestros
        public ActionResult ImportacionMaestros(HttpPostedFileBase Maestro)
        {

            Response response = new BE.Response();
            List<Response> ListImport = new List<Response>();

            if (Maestro != null && Maestro.ContentLength > 0)
            {
                string[] filename = Maestro.FileName.Split('.');
                string extension = filename[1];
                //if (extension != "xls" && extension != "xlsx"&& extension != "XLS" && extension != "XLSX")
                //{
                //    response.HUBO_ERROR = true;
                //    response.MENSAJE_ERROR = "La extensión del archivo no es valida.";
                //    return Json(response);
                //}

                using (var stream = Maestro.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });
                        //List<ImportBE> ListaResult = new List<ImportBE>();
                        // Obtener la tabla del DataSet (asumiendo que es la primera)
                        DataTable dtAlmacen = dataSet.Tables[0];
                        DataTable dtUsuarios = dataSet.Tables[1];
                        DataTable dtUsuXAlmacen = dataSet.Tables[2];
                        DataTable dtProductos = dataSet.Tables[3];
                        DataTable dtUbicaciones = dataSet.Tables[4];
                        //int totalRecords = dtAlmacen.Rows.Count;


                        //if (dtAlmacen.Rows.Count > 0) {
                        ListImport.Add(Import_Maestro_Almacen(dtAlmacen));
                        ListImport.Add(Import_Maestro_Usuarios(dtUsuarios));
                        ListImport.Add(Import_Maestro_UsuXAlmacen(dtUsuXAlmacen));
                        ListImport.Add(Import_Maestro_Productos(dtProductos));
                        ListImport.Add(Import_Maestro_Ubicacion(dtUbicaciones));
                        //}

                        response.Entity = ListImport;
                    }
                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se seleccionó ningún archivo o el archivo está vacío.";
                return Json(response);
            }

            return Json(response);
        }
        public Response Import_Maestro_Almacen(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Almacen";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarAlmacen_Maestro(xmlData);
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarAlmacen_Maestro(xmlDataBlock));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de ALMACEN.";
                return response;
            }
            response.Entity = ListaResult;
            return response;
        }

        public Response Import_Maestro_Usuarios(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Usuario";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarUsuarios_Maestro(xmlData, Session["UserName"].ToString());
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarUsuarios_Maestro(xmlDataBlock, Session["UserName"].ToString()));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de USUARIOS.";
                return response;
            }
            response.Entity = ListaResult;
            return response;
        }

        public Response Import_Maestro_UsuXAlmacen(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "UsuarioXAlmacen";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarUsuariosXAlmacen_Maestro(xmlData);
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarUsuariosXAlmacen_Maestro(xmlDataBlock));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de USUXALMN.";
                return response;
            }
            response.Entity = ListaResult;

            return response;
        }

        public Response Import_Maestro_Productos(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Table1";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarProductos(xmlData);
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarProductos(xmlDataBlock));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de PRODUCTOS.";
                return response;
            }
            response.Entity = ListaResult;

            return response;
        }

        public Response Import_Maestro_Ubicacion(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Ubicacion";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarUbicacion_Maestro(xmlData);
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarUbicacion_Maestro(xmlDataBlock));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de UBICACIONES.";
                return response;
            }
            response.Entity = ListaResult;

            return response;
        }

        #endregion

        #region Importacion de Archivos Inventario
        public ActionResult ImportacionInventario(HttpPostedFileBase Inventario)
        {

            Response response = new BE.Response();
            List<Response> ListImport = new List<Response>();

            if (Inventario != null && Inventario.ContentLength > 0)
            {
                string[] filename = Inventario.FileName.Split('.');
                string extension = filename[1];
                //if (extension != "xls" && extension != "xlsx"&& extension != "XLS" && extension != "XLSX")
                //{
                //    response.HUBO_ERROR = true;
                //    response.MENSAJE_ERROR = "La extensión del archivo no es valida.";
                //    return Json(response);
                //}

                using (var stream = Inventario.InputStream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        });
                        //List<ImportBE> ListaResult = new List<ImportBE>();
                        // Obtener la tabla del DataSet (asumiendo que es la primera)
                        DataTable dtInventario = dataSet.Tables[0];
                        DataTable dtDetInventario = dataSet.Tables[1];

                        //int totalRecords = dtAlmacen.Rows.Count;


                        //if (dtAlmacen.Rows.Count > 0) {
                        Response ResponseValid = new Response();
                        ResponseValid = Import_Inventario_Inventario(dtInventario);
                        ListImport.Add(ResponseValid);

                        List<ImportBE> auxImport = (List<ImportBE>)ResponseValid.Entity;
                        ListImport.Add(Import_Inventario_DetalleInventario(dtDetInventario, auxImport[0].Flg_pass));


                        response.Entity = ListImport;
                    }
                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se seleccionó ningún archivo o el archivo está vacío.";
                return Json(response);
            }

            return Json(response);
        }

        public Response Import_Inventario_Inventario(DataTable dt)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Inventario";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarInventario_Inventario(xmlData, Session["UserName"].ToString());
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarInventario_Inventario(xmlDataBlock, Session["UserName"].ToString()));

                        startIndex += blockSize;
                    }

                }
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de ALMACEN.";
                return response;
            }
            response.Entity = ListaResult;
            return response;
        }

        public Response Import_Inventario_DetalleInventario(DataTable dt, int pass)
        {
            Response response = new BE.Response();
            List<ImportBE> ListaResult = new List<ImportBE>();

            //Calcular el número total de registros
            int totalRecords = dt.Rows.Count;

            if (totalRecords > 0)
            {
                // Determinar si es necesario dividir el XML en bloques
                bool splitXml = totalRecords > 100;

                // Si no es necesario dividir el XML, enviarlo directamente
                dt.TableName = "Table1";
                if (!splitXml)
                {
                    string xmlData = ConvertDataTableToXml(dt);
                    var dataAccess = new MantenimientosBL();
                    ListaResult = dataAccess.ImportarDetInventario_Inventario(xmlData, Session["UserName"].ToString(), pass);
                }
                else
                {
                    // Dividir el XML en bloques de 100 registros
                    int startIndex = 0;
                    int blockSize = 100;

                    while (startIndex < totalRecords)
                    {
                        // Obtener el bloque actual de registros
                        var blockRows = dt.AsEnumerable()
                            .Skip(startIndex)
                            .Take(blockSize)
                            .CopyToDataTable();


                        // Convertir el bloque de registros a XML
                        string xmlDataBlock = ConvertDataTableToXml(blockRows);

                        // Enviar el bloque de registros a ImportarEmpleados
                        var dataAccess = new MantenimientosBL();
                        ListaResult.AddRange(dataAccess.ImportarDetInventario_Inventario(xmlDataBlock, Session["UserName"].ToString(), pass));

                        startIndex += blockSize;
                    }

                }
                new MantenimientosBL().Insert_ASF_DETALLE_INVENTARIO(Session["UserName"].ToString());
            }
            else
            {
                response.HUBO_ERROR = true;
                response.MENSAJE_ERROR = "No se encuentran registros en la hoja de ALMACEN.";
                return response;
            }
            response.Entity = ListaResult;
            return response;
        }


        [HttpPost]
        public JsonResult Select_DET_INV_IMPORT(string start, string length, int draw, string searchValue)
        {
            string sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault().ToString();
            string sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault().ToString();
            var sortDirection = sortColumnDirection.ToString() == "a" ? "asc" : "desc";
            int sortColumnIndexfix = (Int32.Parse(sortColumnIndex)) + 1;
            string order = sortColumnIndexfix.ToString() + ' ' + sortDirection;

            //var search = Request.Form["search[value]"].FirstOrDefault();


            var response = new MantenimientosBL().Select_DET_INV_IMPORT(start, length, order, searchValue);
            List<ImportBE> lista = new List<ImportBE>();
            lista = (List<ImportBE>)response.Entity;
            //var pagedData = lista.Skip(0).Take(10).ToList();
            var totalData = response.count;
            return Json(new { draw = draw, recordsFiltered = totalData, recordsTotal = totalData, data = lista });

        }

        #endregion
        public string ConvertDataTableToXml(DataTable dataTable)
        {
            DataTable clonedDataTable = dataTable.Clone();
            foreach (DataRow row in dataTable.Rows)
            {
                clonedDataTable.ImportRow(row);
            }
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(clonedDataTable);
            using (StringWriter stringWriter = new StringWriter())
            {
                dataSet.WriteXml(stringWriter);
                return stringWriter.ToString();
            }
        }

    }
}