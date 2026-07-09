/*// TRAE LOS ALMACENES //*/
function LoadCbxAlmacenInv() {

    $("#cbxAlmacen_NewInv").empty();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: 'FillCbxAlmacenInv',
        data: '',
        async: false,
        success: function (response) {

            let opcion = "";
            /*row += "<option value=''>--Seleccione--</option>";*/
            if (response.length === 0) {
                opcion += "<option value='-1'>--No se encuentran almacenes--</option>";
                // ALERTA PARA REDIRECCIONAR A LA PAGINA DE MANTENIMIENTO ALMACEN
            } else {
                $.each(response, function (i, item) {
                    opcion += "<option value='" + item.intValue + "'>" + item.vchdesc + "</option>";
                });
            }

            //$("#cbxAlmacen_NewInv").html(opcion);
            $("#cbxAlmacen_NewInv").html(opcion).trigger('change.select2');

            $("#cbxAlmacen_NewInv").select2({
                width: '100%',
                language: 'es',
                placeholder: "--Seleccione un almacen--"
            });
        },
        error: function (result) {
            Swal.fire("Error inesperado", "Ocurrió un problema al procesar la solicitud. Intente nuevamente.", "error");
        }
    });
}



/*// TRAE LAS UBICACIONES //*/
function LoadCbxUbiInv(Id_Almacen) {

    $("#cbxUbicacion_NewInv").empty();
    var obj = new Object();
    obj.id_almacen = Id_Almacen;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: 'FillCbxUbicacionInv',
        data: JSON.stringify(obj),

        async: false,
        success: function (response) {
            //console.log(response);
            let opcion = "";
            /*row += "<option value=''>--Seleccione--</option>";*/
            if (response.length === 0) {
                opcion += "<option value='-1' disabled selected>--No se encuentran ubicaciones asociadas al almacen--</option>";
                // ALERTA PARA REDIRECCIONAR A LA PAGINA DE MANTENIMIENTO UBICACIONES
            } else {
                $.each(response, function (i, item) {
                    opcion += "<option value='" + item.vchValue + "'>" + item.vchdesc + "</option>";
                    /*console.log(item.vchdesc);*/
                });
            }

            //$("#cbxUbicacion_NewInv").html(opcion);
            $("#cbxUbicacion_NewInv").html(opcion).trigger('change.select2');
            $("#cbxUbicacion_NewInv").select2({
                width: '100%',
                language: 'es',
                placeholder: "--Seleccione una ubicación--"
            });
        },
        error: function (result) {
            console.log('error' + result);
            // ALERTA PARA ERROR INESPERADO
        }
    });
}



/*// TRAE LOS PRODUCTOS //*/
function LoadCbxProdInv(dscProd) {

    $("#cbxProducto_NewInv").empty();

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: 'FillCbxProductoInv',
        data: JSON.stringify({ dsc_prod: dscProd }),

        async: false,
        success: function (response) {
            var row = "";
            /*row += "<option value=''>--Seleccione--</option>";*/
            if (response.length === 0) {
                row += "<option value='-1' idUM='-1' >--No se encuentran productos--</option>";
            } else {               
                $.each(response, function (i, item) {
                    row += "<option  value='" + item.vchValue + "' idUM='" + item.intaddValue + "'>" + item.vchdesc + "</option>";
                });
                //console.log(response.length);
            }
            //Console.log(row);
            $("#cbxProducto_NewInv").html(row);
        },
        error: function (result) {
            console.log('error' + result);
        }
    });
}




/*// AL SELECCIONAR UN ALMACEN TRAE SUS UBICACIONES //*/
//document.addEventListener("change", function (e) {

//    if (e.target.id === "cbxAlmacen_NewInv") {
//        let idAlmacen = e.target.value;
//        //console.log(idAlmacen);
//        LoadCbxUbiInv(idAlmacen);

//        e.target.classList.remove("is-invalid");

//        // Valida la seleccion automatica es valida
//        const ubicacionSeleccionada = document.getElementById("cbxUbicacion_NewInv");

//        if (!ubicacionSeleccionada) return;

//        if (ubicacionSeleccionada.length === 0 || ubicacionSeleccionada.value === "-1" || ubicacionSeleccionada.value === "") {
//            inputInvalido(ubicacionSeleccionada, "Este campo es requerido");
//        }
//        else {
//            limpiarInvalido(ubicacionSeleccionada);
//        }

//    }
//});
$(document).on("select2:select", "#cbxAlmacen_NewInv", function (e) {
    limpiarInvalido(this);
    let idAlmacen = $(this).val();
    //console.log(e.params.data.id);

    LoadCbxUbiInv(idAlmacen);

    this.classList.remove("is-invalid");

    const ubicacionSeleccionada = document.getElementById("cbxUbicacion_NewInv");

    if (!ubicacionSeleccionada) return;

    if (ubicacionSeleccionada.length === 0 ||
        ubicacionSeleccionada.value === "-1" ||
        ubicacionSeleccionada.value === "") {

        inputInvalido(ubicacionSeleccionada, "Este campo es requerido");
    }
    else {
        limpiarInvalido(ubicacionSeleccionada);
    }

});



/*// TRAE LAS UNIDADES DE MEDIDA //*/
function LoadUndMed() {
    var idUM = $('option:selected', $('#cbxProducto_NewInv')).attr('idum');

    let _url = '../Mantenimiento/GetUnidadMedida';
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify({ id: idUM }),
        async: false,
        success: function (response) {
            let entity;
            let ValMult;

            //const icon = document.querySelector('#infoUM');
            //const tooltip = bootstrap.Tooltip.getOrCreateInstance(icon);

            if (idUM == -1) {
                $('#UMText').text(''); ValMult = 1
                ValMult = 1;
                //tooltip.setContent({
                //    '.tooltip-inner': 'Unidad'
                //});
            }
            else {
                entity = response.Entity;
                ValMult = entity.IntCantidad;
                $('#UMText').text('Unidad: ' + entity.vchDesUniMed + ' (x' + entity.IntCantidad + ")")
                //tooltip.setContent({
                //    '.tooltip-inner': `Unidad: ${entity.vchDesUniMed} (x${entity.IntCantidad})`
                //});
            }

            $('#UMText').attr('mult', ValMult);
            /*icon.setAttribute('mult', ValMult);*/
        },
        error: function (result) {
            //Console.log('error' + result);
        }
    });
}


/*// INICIALIZAR LA TABLA CREAR INVENTARIO //*/
let tablaProductos;
let estaSelecProd=false;

function InicializarTablaCrearInv() {
    if (!tablaProductos) {
        tablaProductos = $('#tbl_addProductos').DataTable({
            "paging": true,
            "pageLength": 10,
            "searching": true,
            "lengthChange": true,
            "language": españolTbl,
            select: {
                style: 'single'
            },
            deferRender:true,
        });
    }

    /*// SELECCIONAR UNA FILA DE LA TABLA //*/
    tablaProductos.on('click', 'tbody tr', (e) => {
        let classList = e.currentTarget.classList;

        if (classList.contains('selected')) {
            classList.remove('selected');
            estaSelecProd = false;
            return;
        }
        $('#tbl_addProductos').DataTable().rows('.selected').nodes().each((row) => row.classList.remove('selected'));
        classList.add('selected');
        estaSelecProd = true;
    });
}



/*// AGREGAR UN PRODUCTO A LA TABLA //*/
function AgregarNuevoProducto() {
    var CodInv = $('#txt_New_CodInventario').val();

    var Codubi = $('#cbxUbicacion_NewInv').val();
    var Dscubi = $('#cbxUbicacion_NewInv option:selected').text();

    var Codprod = $('#cbxProducto_NewInv').val();
    var Dscprod = $('#cbxProducto_NewInv option:selected').text();

    var Lote = $('#txtLote_NewInv').val();
    var Serie = $('#txtSerie_NewInv').val();
    var Stock = document.getElementById("txtStock_NewInv").value;

    /*$('#txtStock_NewInv').val();*/
    //var IntStock = Stock.value.replace(/[.,]/g, '');
    var umMult = $('#UMText').attr('mult');

    let dataActual = tablaProductos.rows().data().toArray();

    dataActual.unshift([
        CodInv,
        Codubi,
        Dscubi,
        Codprod,
        Dscprod,
        Lote,
        (Math.round(Stock * 1000) / 1000) * umMult,
    ]);

    tablaProductos.clear();
    tablaProductos.rows.add(dataActual).draw();
    //let nuevoProducto = tablaProductos.row
    //    .add([
    //        CodInv,
    //        Codubi,
    //        Dscubi,
    //        Codprod,
    //        Dscprod,
    //        Lote,

    //        (Math.round(Stock * 1000) / 1000) * umMult,
    //        /*Math.trunc(Stock),*/

    //    ])
    //    .draw(false)
    //    .node();

    //$(nuevoProducto).prependTo('#tbl_addProductos tbody');
}



/*// ELIMINAR PRODUCTO A LA TABLA //*/
document.addEventListener("click", (e) => {
    if (e.target.id === "btnEliminarFila") {
        /*console.log(estaSelecProd);*/
        if (!estaSelecProd) {
            Swal.fire({
                icon: 'warning',
                title: 'Selecciona un Producto',
                text: 'Debe seleccionar una fila de la tabla antes de eliminar un producto.',
            });

            return;
        }

        tablaProductos.row(".selected").remove().draw(false);
        estaSelecProd = false;
    }
});



/*// IMPORTAR PRODUCTOS A LA TABLA //*/
function ImportarProductos(data) {
    var CodInv = $('#txt_New_CodInventario').val();

    let rowsToAdd = [];

    data.forEach(item => {
        if (item.Flg_Pass === 1) {
            rowsToAdd.push([
                CodInv,
                item.COD_UBICACION,
                item.DSC_UBICACION,
                item.COD_PRODUCTO,
                item.DSC_PRODUCTO,
                item.LOTE_PRODUCTO,
                Math.trunc(item.STOCK_ACTUAL),
            ]);
        }
    });

    tablaProductos.rows.add(rowsToAdd).draw(false);
}



/*// MOSTRAR PRODUCTOS NO IMPORTADOS EN EL MODAL //*/
let tablaNoImportados;
function InicializarTablaNoImport() {
    if (!tablaNoImportados) {
        tablaNoImportados = $('#RImport_Tbl').DataTable({
            "paging": true,
            "pageLength": 10,
            "searching": false,
            "lengthChange": true,
            "language": españolTbl,
            select: {
                style: 'single'
            },
            deferRender: true,
            "columns": [
                { "data": "COD_UBICACION", "title": "Cod. Ubicación" },
                { "data": "DSC_UBICACION", "title": "Dsc. Ubicación" },

                { "data": "COD_PRODUCTO", "title": "Cod. Producto" },
                { "data": "DSC_PRODUCTO", "title": "Dsc. Producto" },

                { "data": "LOTE_PRODUCTO", "title": "Lote Producto" },
                { "data": "STOCK_ACTUAL", "title": "Stock" },
                { "data": "Desc_Error", "title": "Mensaje" },
                {
                    "data": null,
                    "orderable": false,
                    "searchable": false,
                    "title": "Resultado",
                    "render": () => '<i class="ti ti-x text-danger"></i>'
                }
            ]
        });
    }
}

function MostrarTablaNoImportados(Jsondata) {
    InicializarTablaNoImport();

    tablaNoImportados.clear();
    tablaNoImportados.rows.add(Jsondata);
    tablaNoImportados.draw();
}



/*// GUARDAR EL INVENTARIO PREPARADO //*/
function GuardarInventarioPreparado() {
    ////Console.log($('#tbl_addProductos').DataTable().rows().data());
    if (tablaProductos.rows().count() <= 0) {

        Swal.fire("No hay productos", "Debe ingresar productos en la tabla", "warning");
        //$('#txtvarError').text('Debe ingresar registros en la tabla');
        //$('#error_modal').modal('show');
        return;
    }

    //var data = tablaProductos.rows().data().toArray();
    //var xml = '<NewDataSet>';

    //data.forEach(function (row) {
    //    xml += '<Table1>';
    //    xml += '<Cod_Inventario>' + row[0] + '</Cod_Inventario>';
    //    xml += '<Cod_Ubicacion>' + row[1] + '</Cod_Ubicacion>';
    //    xml += '<Cod_Producto>' + row[3] + '</Cod_Producto>';
    //    xml += '<Lote_Producto>' + row[5] + '</Lote_Producto>';
    //    /*xml += '<Serie_Producto>' + row[6] + '</Serie_Producto>';*/
    //    xml += '<Stock_Actual>' + row[6] + '</Stock_Actual>';
    //    xml += '</Table1>';
    //});

    //xml += '</NewDataSet>';

    var CodInv = $('#txt_New_CodInventario').val();
    var Id_Almacen = $('#cbxAlmacen_NewInv').val();

    var obj = new Object();
    //obj.xmlData = xml; YA NO ENVIO XML
    obj.importacionId = importacionId;
    obj.CodInv = CodInv;
    obj.Id_Almacen = Id_Almacen;

    console.log(obj);
    //var formData = new FormData();
    //formData.append("xmlData", xml);
    ///*formData.append("xmlData", "hola");*/
    //formData.append("CodInv", CodInv);
    //formData.append("Id_Almacen", Id_Almacen);

    let _url = 'InsertInv_InvDetalle';

    //console.log(xml.length);
    //alert((xml.length / 1024 / 1024).toFixed(2) + " MB");

    $.ajax({
        url: _url,
        type: 'POST',
        /*data: { xmlData: xml, CodInv: CodInv, Id_Almacen: Id_Almacen },*/
        data: JSON.stringify(obj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",

        success: function (response) {
            if (response.HUBO_ERROR) {
                //Console.log(response);
                //$('#txtvarError').text(response.MENSAJE_ERROR);
                //$('#error_modal').modal('show');
                Swal.fire("Error", response.MENSAJE_ERROR, "error");
            }
            else {
                Swal.fire({
                    title: "Inventario preparado correctamente",
                    text: "Listo para gestionarlo",
                    icon: "success",
                    allowOutsideClick: false,  // impedir cerrar con click fuera
                    allowEscapeKey: false,     // impedir cerrar con ESC
                    showCloseButton: false,    // sin botón X
                    confirmButtonText: "Ir a Gestionar"
                }).then(() => {
                    window.location.href = urlGestionar;
                });
                //$('#txtvar').text(response.MENSAJE_ERROR);
                //$('#success_modal').modal('show');
                //CerrarNewInv();
                //$('#success_modal').on('hidden.bs.modal', function () {
                //    location.reload();
                //});
                //LoadCbxInventario();
            }
        },
        error: function (error) {
            Swal.fire("Error inesperado", "Ocurrió un problema al procesar la solicitud. Intente nuevamente.", "error");
            //$('#txtvarError').text('Error Solicitud Ajax');
            //$('#error_modal').modal('show');
        }
        //error: function (xhr, status, error) {

        //    console.log(xhr.status);
        //    console.log(status);
        //    console.log(error);
        //    console.log(xhr.responseText);

        //    Swal.fire({
        //        icon: "error",
        //        title: "Error",
        //        html:
        //            "<b>HTTP:</b> " + xhr.status +
        //            "<br><b>Status:</b> " + status +
        //            "<br><b>Error:</b> " + error
        //    });
        //}
    });
}



// VALIDACION PASO 1
function validarCamposLlenados(form) {

    const inputsPaso1 = form.querySelectorAll("#txt_New_CodInventario, #cbxAlmacen_NewInv");
    //console.log(inputsPaso1);
    let esValido = true;

    inputsPaso1.forEach(input => {
        input.classList.remove("is-invalid", "is-valid");

        if (input.value.trim() === "" || input.value === "-1") {
            /*input.value = "";*/
            input.classList.add("is-invalid");
            esValido = false;
        } else {
            input.classList.add("is-valid");
        }
    });

    return esValido;
}



/*// VALIDAR DUPLICIDAD DE CODIGO DE INVENTARIO //*/
function validarExistenciaInv() {
    let CodInv = document.getElementById("txt_New_CodInventario");
    let CodAlm = document.getElementById("cbxAlmacen_NewInv");

    let esValido = true;

    var obj = new Object();
    obj.Cod_inventario = CodInv.value;
    obj.Id_Almacen = CodAlm.value;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: 'ValidarBloque1',
        data: JSON.stringify(obj),
        async: false,
        success: function (response) {
            ////Console.log(response);
            if (response.HUBO_ERROR) {

                const msg = response.MENSAJE_ERROR.toLowerCase();
                esValido = false;

                if (msg.includes("codigo invalido")) {
                    inputInvalido(CodInv, response.MENSAJE_ERROR);
                }
                else if (msg.includes("almacen con inventario en proceso")) {
                    inputInvalido(CodAlm, "Almacen seleccionado con inventario en proceso")
                    Swal.fire("Inventario en proceso", response.MENSAJE_ERROR, "warning");
                }
                else {
                    Swal.fire("Error", response.MENSAJE_ERROR, "error");
                }
            }
        },
        error: function (result) {
            // ALERTA DE ERROR
            Swal.fire("Error inesperado", "Ocurrió un problema al procesar la solicitud. Intente nuevamente.", "error");
            esValido = false;
        }
    });

    return esValido;
}



/*// VALIDACION AL GUARDAR ARCHIVO EXCEL //*/
const extensionesPermitidas = ["xlsx", "xls"];

CrearInv_fileInput.addEventListener("change", () => {
    validarArchivoInventario(inputInventarioFile);
});

function validarArchivoInventario(inputFile) {
    inputFile.classList.remove("is-valid", "is-invalid");


    if (inputFile.files.length === 0) {
        inputFile.classList.add("is-invalid");
        return false;
    }

    const file = inputFile.files[0];
    const nombreFile = file.name.toLowerCase();
    const extensionFile = nombreFile.split(".").pop();

    if (!extensionesPermitidas.includes(extensionFile)) {
        inputFile.classList.add("is-invalid");
        return false;
    }

    inputFile.classList.add("is-valid");
    return true;
}