//$("#NewInvPrincipal_btnClose").on("click", function () {
//    CerrarNewInv();
//});
//function CerrarNewInv() {
//    $('.Inv_grid_btn').css('visibility', 'visible');
//    $('#btnCrearInventarioAPI').css('display', 'block');
//    $('#NewInvPrincipal').css('display', 'none');
//    $('#txt_New_CodInventario').val('');
//    BloquearBloque2(true);
//    $('#txtLote_NewInv').val('');
//    $('#txtSerie_NewInv').val('');
//    $('#txtStock_NewInv').val(0);
//    deleteAllRows();
//}

//document.getElementById("btnCrearInventario").onclick = function () {
//    console.log(document.getElementById("btnCrearInventario"));
//    ////Console.log('click');
//    GestorNewInventario();
//    CreateINV();
//};
//function LoadCbxAlmacenInv() {


//    $("#cbxAlmacen_NewInv").empty();
//    let _url = 'FillCbxAlmacenInv';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: '',
//        async: false,
//        success: function (response) {
//            ////Console.log(response);
//            var row = "";
//            /*row += "<option value=''>--Seleccione--</option>";*/
//            if (response.length === 0) {
//                row += "<option value='-1'>--No se encuentran almacenes--</option>";

//            } else {
//                $.each(response, function (i, item) {
//                    row += "<option value='" + item.intValue + "'>" + item.vchdesc + "</option>";
//                });
//            }
//            /*//Console.log(row);*/
//            $("#cbxAlmacen_NewInv").html(row);
//        },
//        error: function (result) {
//            //Console.log('error' + result);
//        }
//    });


//}

//function LoadCbxUbiInv(Id_Almacen) {

//    $("#cbxUbicacion_NewInv").empty();
//    var obj = new Object();
//    obj.id_almacen = Id_Almacen;

//    let _url = 'FillCbxUbicacionInv';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),

//        async: false,
//        success: function (response) {
//            ////Console.log(response);
//            var row = "";
//            /*row += "<option value=''>--Seleccione--</option>";*/
//            if (response.length === 0) {
//                row += "<option value='-1'>--No se encuentran ubicaciones asociadas al almacen--</option>";
//            } else {
//                $.each(response, function (i, item) {
//                    row += "<option value='" + item.vchValue + "'>" + item.vchdesc + "</option>";
//                });
//            }
//            /*//Console.log(row);*/
//            $("#cbxUbicacion_NewInv").html(row);
//        },
//        error: function (result) {
//            //Console.log('error' + result);
//        }
//    });
//}




// Evento para cuando se abre el desplegable de Select2
//$('#cbxProducto_NewInv').on('select2:open', function (e) {
//    const searchField = $('.select2-search__field')[0];
//    if (searchField) {
//        // Evento keyup en el campo de búsqueda
//        searchField.addEventListener('keyup', function () {
//            const term = $(this).val();
//            if (term.length >= 2) { // O la cantidad de caracteres que desees para activar la búsqueda
//                LoadCbxProdInv(term);
//            } else if (term.length === 0) {
//                // Opcional: Puedes recargar el combo con todos los valores o dejarlo vacío
//                // LoadCbxProdInv('');
//                $("#cbxProducto_NewInv").empty().append('<option value="">-- Escribe para buscar --</option>');
//            }
//        });
//    }
//});

//function StartcbxProducto_NewInv() {
//    $("#cbxProducto_NewInv").html('<option value="">-- Escribe para buscar --</option>');
//}

//function LoadCbxProdInv(dscProd) {


//    $("#cbxProducto_NewInv").empty();
//    let _url = 'FillCbxProductoInv';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify({ dsc_prod: dscProd }),

//        async: false,
//        success: function (response) {
//            ////Console.log(response);
//            var row = "";
//            /*row += "<option value=''>--Seleccione--</option>";*/
//            if (response.length === 0) {
//                row += "<option value='-1' idUM='-1' >--No se encuentran productos--</option>";
//            } else {
//                $.each(response, function (i, item) {
//                    row += "<option  value='" + item.vchValue + "' idUM='" + item.intaddValue +"'>" + item.vchdesc + "</option>";
//                });
//            }
//            /*//Console.log(row);*/
//            $("#cbxProducto_NewInv").html(row);
//        },
//        error: function (result) {
//            //Console.log('error' + result);
//        }
//    });


//}
//function GestorNewInventario() {
//    $('.Inv_grid_btn').css('visibility', 'hidden');
//    $('#btnCrearInventarioAPI').css('display', 'none');

//    $('#NewInvPrincipal').css('visibility', 'visible');
//    $('#NewInvPrincipal').css('display', 'block');
//    LoadCbxAlmacenInv();
//    //LoadCbxUbiInv();

//    StartcbxProducto_NewInv();
//    LoadCbxProdInv('');

//    BeginTableProductos()
//}

//function ValidBloque1() {
//    var CodInv = $('#txt_New_CodInventario').val();
//    var CodAlm = $('#cbxAlmacen_NewInv').val();
//    var pass = true;
//    if (CodInv == '' || CodAlm == -1) {
//        pass = false;
//        $('#txtvarError').text("Datos no validos");
//        $('#error_modal').modal('show');
//        return pass;
//    }
//    var obj = new Object();
//    obj.Cod_inventario = CodInv;
//    obj.Id_Almacen = CodAlm;

//    let _url = 'ValidarBloque1';

//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),
//        async: false,
//        success: function (response) {
//            ////Console.log(response);
//            if (response.HUBO_ERROR == true) {
//                pass = false;
//                $('#txtvarError').text(response.MENSAJE_ERROR);
//                $('#error_modal').modal('show');
//            }

//        },
//        error: function (result) {

//            $('#txtvarError').text('Error Solicitud Ajax');
//            $('#error_modal').modal('show');
//        }
//    });

//    return pass;
//}

//function BloquearBloque2(flg) {
//    if (flg) {
//        $('.input_bloque2').prop('disabled', true);
//        $('.lbl_bloque2').addClass('backDisabled');

//        $('.input_bloque1').prop('disabled', false);
//        $('.lbl_bloque1').removeClass('backDisabled');
//    } else {
//        if (ValidBloque1()) {
//            $('.input_bloque2').prop('disabled', false);
//            $('.lbl_bloque2').removeClass('backDisabled');

//            $('.input_bloque1').prop('disabled', true);
//            $('.lbl_bloque1').addClass('backDisabled');

//        }


//    }
//}

//function ValidBloque2() {
//    var Id_ubi = $('#cbxUbicacion_NewInv').val();
//    var Id_prod = $('#cbxProducto_NewInv').val();
//    var Lote = $('#txtLote_NewInv').val();
//    var Serie = $('#txtSerie_NewInv').val();
//    var Stock = $('#txtStock_NewInv').val();
//    var pass = true;
//    if (Id_ubi == -1 || Id_prod == -1 || Stock <= 0) {
//        pass = false;
//        $('#txtvarError').text("Datos no validos");
//        $('#error_modal').modal('show');
//    }

//    return pass;
//}

//function addNewRow() {
//    var CodInv = $('#txt_New_CodInventario').val();

//    var Codubi = $('#cbxUbicacion_NewInv').val();
//    var Dscubi = $('#cbxUbicacion_NewInv option:selected').text();

//    var Codprod = $('#cbxProducto_NewInv').val();
//    var Dscprod = $('#cbxProducto_NewInv option:selected').text();
//    var Lote = $('#txtLote_NewInv').val();
//    var Serie = $('#txtSerie_NewInv').val();
//    var Stock = document.getElementById("txtStock_NewInv").value;
//    /*$('#txtStock_NewInv').val();*/
//    //var IntStock = Stock.value.replace(/[.,]/g, '');
//    var umMult =$('#UMText').attr('mult');

//    $('#tbl_addProductos').DataTable().row
//        .add([
//            CodInv,
//            Codubi,
//            Dscubi,
//            Codprod,
//            Dscprod,
//            Lote,

//            (Math.round(Stock * 1000) / 1000) * umMult,
//            /*Math.trunc(Stock),*/

//        ])
//        .draw(false);


//}

function deleteAllRows() {
    var rows = $('#tbl_addProductos').DataTable()
        .rows()
        .remove()
        .draw();
}
//function BeginTableProductos() {
//    $('#tbl_addProductos').DataTable();
//    deleteAllRows();
//}
//document.getElementById("btnConfirmarBloque2").onclick = function () {
//    if (ValidBloque1() && ValidBloque2()) {
//        addNewRow()
//    }

//};

//$('#tbl_addProductos').DataTable().on('click', 'tbody tr', (e) => {
//    let classList = e.currentTarget.classList;

//    if (classList.contains('selected')) {
//        classList.remove('selected');
//    }
//    else {
//        $('#tbl_addProductos').DataTable().rows('.selected').nodes().each((row) => row.classList.remove('selected'));
//        classList.add('selected');
//    }
//});

//document.getElementById("btnEliminarFila").onclick = function () {
//    var rows = $('#tbl_addProductos').DataTable()
//        .rows('.selected')
//        .remove()
//        .draw();
//};

//document.getElementById("btnConfirmarBloque1").onclick = function () {
//    BloquearBloque2(false);
//    var idAlmacen = $('#cbxAlmacen_NewInv').val();
//    LoadCbxUbiInv(idAlmacen);
//};

//document.getElementById("btnGuardarInventario").onclick = function () {
//    ////Console.log($('#tbl_addProductos').DataTable().rows().data());
//    if ($('#tbl_addProductos').DataTable().rows().count() <= 0) {
        
//        $('#txtvarError').text('Debe ingresar registros en la tabla');
//        $('#error_modal').modal('show');
//        return;
//    }
        
//    var data = $('#tbl_addProductos').DataTable().rows().data().toArray();
//    var xml = '<NewDataSet>';

//    data.forEach(function (row) {
//        xml += '<Table1>';
//        xml += '<Cod_Inventario>' + row[0] + '</Cod_Inventario>';
//        xml += '<Cod_Ubicacion>' + row[1] + '</Cod_Ubicacion>';
//        xml += '<Cod_Producto>' + row[3] + '</Cod_Producto>';
//        xml += '<Lote_Producto>' + row[5] + '</Lote_Producto>';
//        /*xml += '<Serie_Producto>' + row[6] + '</Serie_Producto>';*/
//        xml += '<Stock_Actual>' + row[6] + '</Stock_Actual>';
//        xml += '</Table1>';
//    });

//    xml += '</NewDataSet>';

//    var CodInv = $('#txt_New_CodInventario').val();
//    var Id_Almacen = $('#cbxAlmacen_NewInv').val();

//    var obj = new Object();
//    obj.xmlData = xml;
//    obj.CodInv = CodInv;
//    obj.Id_Almacen = Id_Almacen;

//    let _url = 'InsertInv_InvDetalle';
//    $.ajax({
//        url: _url,
//        type: 'POST',
//        /*data: { xmlData: xml, CodInv: CodInv, Id_Almacen: Id_Almacen },*/
//        data: JSON.stringify(obj),
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",

//        success: function (response) {
//            if (response.HUBO_ERROR) {
//                //Console.log(response);
//                $('#txtvarError').text(response.MENSAJE_ERROR);
//                $('#error_modal').modal('show');
//            }
//            else {
//                $('#txtvar').text(response.MENSAJE_ERROR);
//                $('#success_modal').modal('show');
//                CerrarNewInv();
//                $('#success_modal').on('hidden.bs.modal', function () {
//                    location.reload();
//                });
//                //LoadCbxInventario();
//            }
//        },
//        error: function (error) {
//            $('#txtvarError').text('Error Solicitud Ajax');
//            $('#error_modal').modal('show');
//        }
//    });

//};
//function validateNumber(event) {
//    var key = window.event ? event.keyCode : event.which;
//    if (["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"].indexOf(String.fromCharCode(key)) !== -1) {
//        return true;
//    } else {
//        return false;
//    }
//}

//$('#btnExcel_crearInv').on("click", function () {
//    $('#CrearInv_Importar').modal('show');
//});

//function Charge_RImport_Modal(Jsondata) {
//    //Console.log(Jsondata)
//    $('#RImport_modal').modal('show');
//    RImport_tabla = $('#RImport_Tbl').DataTable({
//        "data": Jsondata,
//        "createdRow": function (row, data, dataIndex) {

//            //if (data.Flg_pass == 0) {
//            //    $(row).addClass('RowError');
//            //} else { $(row).addClass('RowSuccess'); }
//        },
//        "columns": [
//            { "data": "COD_UBICACION", "title": "Cod_Ubicación" },
//            { "data": "DSC_UBICACION", "title": "Dsc_Ubicación" },

//            { "data": "COD_PRODUCTO", "title": "Cod_Producto" },
//            { "data": "DSC_PRODUCTO", "title": "Dsc_Producto" },

//            { "data": "LOTE_PRODUCTO", "title": "Lote_Producto" },
//            { "data": "STOCK_ACTUAL", "title": "Stock" },
//            { "data": "Desc_Error", "title": "Mensaje" },
//            {
//                "data": null,
//                "orderable": false,
//                "searchable": false,
//                "title": "Resultado",
//                "render": function (data, type, row, meta) {
//                    var stringResult = '';
//                    if (data.Flg_Pass == 0) {
//                        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
//                    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


//                    return stringResult

//                }
//            }

//        ],
//        "paging": true,
//        "pageLength": 10,
//        "searching": true,
//        "lengthChange": true,
//        "responsive": true,

//        "language": españolTbl
//    });



//}

//function addImport(data) {
//    var CodInv = $('#txt_New_CodInventario').val();
//    //Console.log('asdasdasda');

//    $.each(data, function (index, item) {
//        if (item.Flg_Pass === 1) {
//            $('#tbl_addProductos').DataTable().row
//                .add([
//                    CodInv,
//                    item.COD_UBICACION,
//                    item.DSC_UBICACION,
//                    item.COD_PRODUCTO,
//                    item.DSC_PRODUCTO,
//                    item.LOTE_PRODUCTO,
//                    Math.trunc(item.STOCK_ACTUAL),

//                ])
//                .draw(false);

            
//        }
//    });
//}


//$('#cbxProducto_NewInv').change(function () {
//    CreateINV();
//})

//function CreateINV() {
//    var idUM = $('option:selected', $('#cbxProducto_NewInv')).attr('idum');

//    let _url = '../Mantenimiento/GetUnidadMedida';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify({ id: idUM }),
//        async: false,
//        success: function (response) {
//            let entity;
//            let ValMult; 
//            if (idUM == -1) {
//                $('#UMText').text(''); ValMult = 1
//            }
//            else {
//                entity = response.Entity;
//                ValMult = entity.IntCantidad;
//                $('#UMText').text('UM: ' + entity.vchDesUniMed + ' - Valor: ' + entity.IntCantidad)
//            }

//            $('#UMText').attr('mult', ValMult);
//        },
//        error: function (result) {
//            //Console.log('error' + result);
//        }
//    });
//}