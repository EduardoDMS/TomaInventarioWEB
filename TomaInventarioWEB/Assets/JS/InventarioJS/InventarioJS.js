var init = true;
var Interval = false;
var ConteoActual = 0;
var Global_CodInventario = '';

//$('#cbxInventarioP').change(function () {
//    if ($('#cbxInventarioP').val() != -1) {
//        /*////Console.log($("#cbxInventarioP option:selected").text());*/
//        //AbrirGestor($("#cbxInventarioP option:selected").text());
//        AbrirGestor($("#cbxInventarioP").val());
//    }
//})

//function LoadCbxInventario() {
    
    
//    $("#cbxInventarioP").empty();
//    let _url = 'FillCbxInventario';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: '',
//        async: false,
//        success: function (response) {
//            //////Console.log(response);
//            var row = "";
//            /*row += "<option value=''>--Seleccione--</option>";*/
//            if (response.length === 0) {
//                row += "<option value='-1'>--Sin resultados--</option>";
//            } else {
//                $.each(response, function (i, item) {
//                    row += "<option value='" + item.vchValue + "'>" + item.vchdesc + "</option>";
//                });
//            }
//            /*////Console.log(row);*/
//            $("#cbxInventarioP").html(row);
//        },
//        error: function (result) {
//            ////Console.log('error' + result);
//        }
//    });


//}

let nIntervId;

//function AbrirGestor(Cod_Inventario) {
//    $('#ModalSelectInv').modal('hide');
//    document.getElementById('cbxInventarioP').selectedIndex = 0;
//    $('.Inv_grid_btn').css('visibility', 'hidden');
//    $('#btnCrearInventarioAPI').css('display', 'none');
//    $('#GestorPrincipal').css('visibility', 'visible');
//    $('#GestorPrincipal').css('display', 'block');
//    //Cargar Datos de Inventario
//    CargarDatosInventario(Cod_Inventario);
//    Global_CodInventario = Cod_Inventario;
//    //Cargar Tabla
//    //destroyTable();
//    CargarTablaInventario(Cod_Inventario);                   
//    Interval = true;
    
//        cargarInterval();
    

//    //let reloadtableGestor = parseInt($('#GestorPrincipal').attr("timer"));
//    //if (!nIntervId) {
//    //    nIntervId = setInterval(function () {

//    //        if (Interval) {
//    //            ////Console.log('reecarga auto')
//    //            $('#tbl_Inventario').DataTable().ajax.reload(null, true);
//    //        }
//    //        ////Console.log('Ejecutar Interval')

//    //    }, reloadtableGestor);
//    //}
    
//    //var btnActual = document.getElementById("btnConteo" + ConteoActual);
//    //var btnSiguiente = document.getElementsByClassName('btnconteoSiguiente')[0]
//    //    //document.getElementById("btnConteo" + (ConteoActual + 1));
//    ////Console.log('btnActual: ' + btnActual.id + '- btnSiguiente ' + btnSiguiente.id)
//    //btnActual.onclick = function () {
//    //    reloadTable();
//    //};
//    //if (ConteoActual < 3) {
//    //    //btnSiguiente.classList.add("botonConteo");
//    //    btnSiguiente.onclick = function () {
//    //        $('#ModalConteo').modal('show');
//    //    };
//    //}

//    var btnCerrarInv = document.getElementById("btnCerrarInventario");
//    btnCerrarInv.onclick = function () {
//        CerrarInventario();
//    };
//    OcultarFiltros();
//}
//Datos inventario/////////////////////

//var SwitchchkReload = $("#chkReload");
//SwitchchkReload.change(function () {
//    if (document.getElementById('chkReload').checked) {
//        cargarInterval()
//    } else {
        
//        clearInterval(nIntervId);
//        nIntervId = null;
//    }
    
//});

//function cargarInterval() {
//    let reloadtableGestor = parseInt($('#GestorPrincipal').attr("timer"));
//    if (document.getElementById('chkReload').checked) {
//        ////Console.log('Carga automatica activa');
//        if (!nIntervId) {
//            nIntervId = setInterval(function () {

//                if (Interval) {
//                    ////Console.log('reecarga auto')
//                    $('#tbl_Inventario').DataTable().ajax.reload(null, true);
//                }
//                ////Console.log('Ejecutar Interval')

//            }, reloadtableGestor);
//        }
//    }
    
//}
//function CargarDatosInventario(Cod_Inventario) {
//    let _url = 'GetInventario';
//    var obj = new Object();
//    obj.idEmpresa = '1';
//    obj.Almacen = -1;
//    obj.CodInventario = Cod_Inventario;
//    obj.CodEstado = 'A';
//    obj.flg_filtroFecha = '0';
//    obj.fch_inicio = '';
//    obj.fch_fin = '';
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),
//        async: false,
//        success: function (response) {
//            ConteoActual = response[0].NRO_CONTEO;
//            $('#lblInventario').text(response[0].Nombre);
//            /*////Console.log('conteo Actual: ' + ConteoActual);*/
//            CargarButtonsConteo();
//        },
//        error: function (result) {
//            ////Console.log('error' + result);
//        }
//    });
//}

//function CargarButtonsConteo() {
//    ////Console.log('Conteo: '+ConteoActual)

//    for (i = 1; i <= 3; i++){
//        $('#iconConteo' + i + ' i').removeClass('fa-minus');
//        $('#iconConteo' + i + ' i').removeClass('fa-check');
//        $('#iconConteo' + i + ' i').removeClass('fa-rotate');
//        if (i == ConteoActual) {
//            $('#iconConteo' + i + ' i').addClass('fa-rotate');

//            $('#btnConteo' + i).addClass('btnconteoActual');
//            $('#btnConteo' + i).removeClass('btnconteoSiguiente');
//            $('#btnConteo' + (i+1)).addClass('btnconteoSiguiente');

//        }
//        if (i < ConteoActual) {
//            $('#iconConteo' + i + ' i').addClass('fa-check');

//            $('#btnConteo' + i).removeClass('btnconteoActual');
//            $('#btnConteo' + i).removeClass('btnconteoSiguiente');

//        }
//        if (i > ConteoActual) {
//            $('#iconConteo' + i + ' i').addClass('fa-minus');
//        }
//    }
//    if (ConteoActual >= 2) {
//        $('#btnConteo3').prop('disabled', false);
//    } else { $('#btnConteo3').prop('disabled', true); }
//    if (ConteoActual == 3) {
//        $('#btnCerrarInventario').css('display', 'block');
//    } else { $('#btnCerrarInventario').css('display', 'none'); }

//    //var btnActual = document.getElementById("btnConteo" + ConteoActual);
//    //var btnSiguiente = document.getElementsByClassName('btnconteoSiguiente')[0]
//    ////document.getElementById("btnConteo" + (ConteoActual + 1));
//    ////Console.log('btnActual: ' + btnActual.id + '- btnSiguiente ' + btnSiguiente.id)
//    //btnActual.onclick = function () {
//    //    reloadTable();
//    //};
//    //if (ConteoActual < 3) {
//    //    //btnSiguiente.classList.add("botonConteo");
//    //    btnSiguiente.onclick = function () {
//    //        $('#ModalConteo').modal('show');
//    //    };
//    //}
    

//}

//$("#btnConteo1").on("click", function () {
//    ShowModalConteo(1);
//});
//$("#btnConteo2").on("click", function () {
//    ShowModalConteo(2);
//});
//$("#btnConteo3").on("click", function () {
//    ShowModalConteo(3);
//});

//function ShowModalConteo(N) {
//    var NActual = ConteoActual
//    var NSiguiente = (ConteoActual + 1)
//    if (N == NActual) {
//        reloadTable();
//    }else {
//        if (N == NSiguiente) {
//            $('#ModalConteo').modal('show');
//        }
//    }
//}

//Tabla Inventario///////////////////////
//function destroyTable() {
//    if (!init) {
//        $('#tbl_Inventario').DataTable().destroy();
//        $('#Tbl_reportInventario').DataTable().destroy();
//    }
//    init = false;
//}

//$("#GestorPrincipal_CloseBtn").on("click", function () {
//    CerrarGestor();
//});
//function CerrarGestor() {
//    //$('#tbl_Inventario').DataTable().destroy();
//    $('.Inv_grid_btn').css('visibility', 'visible');
//    $('#btnCrearInventarioAPI').css('display', 'block');

//    //$('#GestorPrincipal').css('visibility', 'hidden');
//    $('#GestorPrincipal').css('display', 'none');
//    Interval = false;
//    clearInterval(nIntervId);
//    nIntervId = null;
//    ConteoActual = 0;
//    Global_CodInventario = '';
//    LoadCbxInventario();
//    habilitarRadioExcel();
//    for (i = 1; i <= 4; i++) {
//        $('#contenedorTblDiv'+i).css('display', 'none');
//    }  
//}

//function CargarTablaInventario(Cod_Inventario) {

//    if (typeof Inventario_tabla !== 'undefined') {
//        Inventario_tabla.destroy();

//    }

//    Inventario_tabla = $('#tbl_Inventario').DataTable({
//        /*"data": Jsondata,*/
//        "serverSide": true,
//        "ajax": {
//            "url": "ListarInventario",
//            "type": "POST",
//            "datatype": "json",
//            "data": function (f) {
//                f.COD_INVENTARIO = Cod_Inventario;
//                f.NRO_CONTEO_1 = 0;
//                f.NRO_CONTEO_2 = 0;
//                f.NRO_CONTEO_3 = 0;
//                f.searchValue = $('input[type="search"][aria-controls="tbl_Inventario"]').val();
//                //f.search
//                ////Console.log(f);
//                toggle_Loadingtb('tbLoading', true);
//            }, "complete": function (response) {
//                //Console.log('tabla:');
//                console.log(response.responseJSON);

//                var codAlmacen = response.responseJSON.data[0].Dsc_Almacen;
//                $('#lblAlmacen').text(codAlmacen);

//                var data = response.responseJSON.data

//                var totalElementos = response.responseJSON.recordsTotal;
//                var InventarioContado = response.responseJSON.Listfooter[6];
//                $('#tblConteoData').text(InventarioContado + '/' + totalElementos);
//                toggle_Loadingtb('tbLoading', false);
//                var faltantes = totalElementos - InventarioContado;
//                CargarCuadrosGraficos(Cod_Inventario, InventarioContado, faltantes, totalElementos);
//                ////Console.log('RESPUESTA:::::');
//                ////Console.log(response);
//                //////Console.log(response.responseJSON.Listfooter[0]);
//                CargarFooter(response.responseJSON.Listfooter[0], response.responseJSON.Listfooter[1], response.responseJSON.Listfooter[2], response.responseJSON.Listfooter[3],
//                    response.responseJSON.Listfooter[4], response.responseJSON.Listfooter[5]);
//                var TStockInicial = $('#FCantI').text();
//                var TConteo1 = $('#FCont1').text();
//                var TConteo2 = $('#FCont2').text();
//                var TConteo3 = $('#FCont3').text();

//                var Inventariado = 0;
//                if (ConteoActual == 1) { Inventariado = TConteo1; }
//                if (ConteoActual == 2) { Inventariado = TConteo2; }
//                if (ConteoActual == 3) { Inventariado = TConteo3; }

                
//                var Faltantes = TStockInicial - Inventariado;

//                DrawnBarChart1(TStockInicial);
//                DrawnBarChart2(TStockInicial);
//                DrawnChart3(Faltantes, Inventariado);

//            }
//        },
//        "createdRow": function (row, data, dataIndex,cells) {
//            if (data.FLG_ESNUEVO === '1') {
//                $(row).addClass('rowYellow');
//            }
//            var cabecera = '#';
//            $('#Head_Cont1').removeClass('headBlue');
//            $('#Head_Cont2').removeClass('headBlue');
//            $('#Head_Cont3').removeClass('headBlue');
//            switch (ConteoActual) {
//                case 1: cabecera = cabecera + 'Head_Cont1'; row.querySelector(':nth-child(6)').classList.add('Font-W_700');  break;
//                case 2: cabecera = cabecera + 'Head_Cont2'; row.querySelector(':nth-child(7)').classList.add('Font-W_700');  break;
//                case 3: cabecera = cabecera + 'Head_Cont3'; row.querySelector(':nth-child(8)').classList.add('Font-W_700');  break;

//                //IQFARMA
//                //case 1: cabecera = cabecera + 'Head_Cont1'; row.querySelector(':nth-child(9)').classList.add('Font-W_700'); break;
//                //case 2: cabecera = cabecera + 'Head_Cont2'; row.querySelector(':nth-child(10)').classList.add('Font-W_700'); break;
//                //case 3: cabecera = cabecera + 'Head_Cont3'; row.querySelector(':nth-child(11)').classList.add('Font-W_700'); break;
//                //FIN IQFARMA
//            }
//            $(cabecera).addClass('headBlue');
            
//            //$(data).addClass('Font-W_700');
            
//            //$(tdActual).addClass('Font-W_700'); 
            
//        },
//        "columns": [
//            { "data": "Cod_Producto", "title": "COD_PRODUCTO" },
//            { "data": "Dsc_Producto", "title": "PRODUCTO" },
//            //IQFARMA

//            //{ "data": "DSC_ANALISIS", "title": "Dsc_Analisis" },
//            //{ "data": "DSC_OBSERVACION", "title": "Dsc_Observación" },
//            //{ "data": "DSC_BALANZA", "title": "Dsc_Balanza" },
//            //FIN IQFARMA
//            { "data": "Cod_ubicacion", "title": "COD_UBICACIÓN" },
//            { "data": "Lote_Producto", "title": "LOTE" },
//            //{ "data": "Serie_Producto", "title": "SERIE" },
//            { "data": "Stock_inicial", "title": "CANTIDAD INICIAL" },
//            { "data": "Conteo_1", "title": "CONTEO 1", "class":"Col_Cont1"},
//            { "data": "Conteo_2", "title": "CONTEO 2", "class":"Col_Cont2" },
//            { "data": "Conteo_3", "title": "CONTEO 3", "class":"Col_Cont3" },
//            { "data": "Stock_Final", "title": "TOTAL" },
//            { "data": "Stock_Diferencial", "title": "DIFERENCIA" }

//        ],
//        "footerCallback": function (row, data, start, end, display) {
          
//            ////var api = this.api();
//            //var TStockInicial = $('#FCantI').text();
//            //var TConteo1 =  $('#FCont1').text();
//            //var TConteo2 =  $('#FCont2').text();
//            //var TConteo3 = $('#FCont3').text();
//            //var TStockFinal = $('#FTotal').text();
//            //var TStockDiferencial = $('#FDif').text();
                
            
            
            
            
//            ////CargarFooter(TStockInicial, TConteo1, TConteo2, TConteo3, TStockFinal, TStockDiferencial);
//            ////var PerTotal = TStockInicial;
//            //var Inventariado = 0;
//            //if (ConteoActual == 1) { Inventariado = TConteo1; }
//            //if (ConteoActual == 2) { Inventariado = TConteo2; }
//            //if (ConteoActual == 3) { Inventariado = TConteo3; }

//            ////PerInventariado = (PerInventariado * 100) / TStockInicial;
//            ////var Perfaltantes = 100 - PerInventariado;
//            ////var ToolTip_Inventariados = PerInventariado;
//            //var Faltantes = TStockInicial - Inventariado;

//            //DrawnBarChart1(TStockInicial);
//            //DrawnBarChart2(TStockInicial);
//            //DrawnChart3(Faltantes, Inventariado );

          
//        },
//        "paging": true,
//        "pageLength": 10,
//        "searching": true,
//        "lengthChange": true,
//        "responsive": false
//        ,

//        "language": españolTbl
//    });
    
    
//}
function ContarLineasTabla(data) {
    var ProductosContados = 0;
    for (var i = 0; i < data.length; i++) {
        if (ConteoActual == 1) { if (data[i].Conteo_1 != 0) { ProductosContados++; } }
        if (ConteoActual == 2) { if (data[i].Conteo_2 != 0) { ProductosContados++; } }
        if (ConteoActual == 3) { if (data[i].Conteo_3 != 0) { ProductosContados++; } }
    }
    return ProductosContados
}
//function reloadTable() {
//    setTimeout(function () {
//        $('#tbl_Inventario').DataTable().ajax.reload(null, true);
//    }, 500);
//}
//function CargarFooter(TStockInicial, TConteo1, TConteo2, TConteo3, TStockFinal, TStockDiferencial) {
//    $('#FCantI').text(TStockInicial);
//    $('#FCont1').text(TConteo1);
//    $('#FCont2').text(TConteo2);
//    $('#FCont3').text(TConteo3);
//    $('#FTotal').text(TStockFinal);
//    $('#FDif').text(TStockDiferencial);
//}
//Opciones de Conteo////////////////////
//$('#ConfirmarConteo').on("click", function () {
//    var radSelect = $('input:radio[name=listGroupRadio]:checked').val();
//    //////Console.log(typeof radSelect);
//    toggle_Loadingtb('tbLoading', true);
//    switch (radSelect) {    
//        case '1': ConteoDiferencial();   break;
//        case '2': ConteoReinicio();  break;
//        case '3': CerrarInventario(); break;
//        default: ////Console.log('Sin selección');; break;
//    }

//});

//function ConteoDiferencial() {
//    let _url = 'ConteoDiferencial'
//    var obj = new Object();
//    obj.codInventario = Global_CodInventario;
//    obj.conteo = ConteoActual;

//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),
//        async: false,
//        success: function (response) {
            
//            if (response.HUBO_ERROR) {
//                $('#txtvarError').text(response.MENSAJE_ERROR);
//                $('#error_modal').modal('show');
//            }
//            else {
//                $('#ModalConteo').modal('hide');
//                $('#txtvar').text(response.MENSAJE_ERROR);
//                $('#success_modal').modal('show');
//                CerrarGestor();
//                var codInventario = $('#lblInventario').text();
//                AbrirGestor(codInventario);
//            }
//        },
//        complete: function (result) {
//            toggle_Loadingtb('tbLoading', false);
//        },
//        error: function (result) {
//            ////Console.log('error' + result);
//        }
//    });

//}
//function ConteoReinicio() {
//    let _url = 'ConteoReinicio'
//    var obj = new Object();
//    obj.codInventario = Global_CodInventario;
//    obj.conteo = ConteoActual;
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),
//        async: false,
//        success: function (response) {
//            if (response.HUBO_ERROR) {
//                $('#txtvarError').text(response.MENSAJE_ERROR);
//                $('#error_modal').modal('show');
//            }
//            else {
//                $('#ModalConteo').modal('hide');
//                $('#txtvar').text(response.MENSAJE_ERROR);
//                $('#success_modal').modal('show');
//                CerrarGestor();
//                var codInventario = $('#lblInventario').text();
//                AbrirGestor(codInventario);
//            }
//        }, complete: function (result) {
//            toggle_Loadingtb('tbLoading', false);
//        },
//        error: function (result) {
//            ////Console.log('error' + result);
//        }
//    });
//}
//function CerrarInventario() {
//    let _url = 'CerrarIventario'
//    var obj = new Object();
//    obj.codInventario = Global_CodInventario;
//    obj.conteo = ConteoActual;
//    $.ajax({
//        type: "POST",
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        url: _url,
//        data: JSON.stringify(obj),
//        async: false,
//        success: function (response) {
//            if (response.HUBO_ERROR) {
//                $('#txtvarError').text(response.MENSAJE_ERROR);
//                $('#error_modal').modal('show');
//            }
//            else {
//                CerrarGestor();
//                $('#ModalConteo').modal('hide');
//                $('#txtvar').text(response.MENSAJE_ERROR);
//                $('#success_modal').modal('show');
//                $('#success_modal').on('hidden.bs.modal', function () {
//                    location.reload();
//                });
//            }
//        }, complete: function (result) {
//            toggle_Loadingtb('tbLoading', false);
//        },
//        error: function (result) {
//            ////Console.log('error' + result);
//        }
//    });
//}

//Modal Usuarios/////////////////////
$(document).ready(function () {
    $('#ModalUsuarios').on('hidden.bs.modal', function () {
        // Destruir DataTable cuando se oculta el modal para la recarga de la tabla
        var tb_user = $('#User_Tbl').DataTable();
        tb_user.destroy();
    });

});
$('#btnUsuarios').on("click", function () {
    var _url = 'ListarUsuariosAsociados';
    var dscAlmacen=$('#lblAlmacen').text();
    var obj = new Object();
    obj.dscAlmacen = dscAlmacen;
    ////Console.log(dscAlmacen);
    
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        async:false,
        success: function (response) {
            User_Modal(response.data);
            $('#ModalUsuarios').modal('show');
        },
        complete: function (response) {

            toggle_Loadingtb('tbLoading', false);
            if (response.HUBO_ERROR) {
                ////Console.log("Error")
            }

        }
    });

    
});

function User_Modal(Jsondata) {
    ////Console.log(Jsondata)
    User_Tbl = $('#User_Tbl').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "Usuario", "title": "Usuario" },
            { "data": "Perfil", "title": "Perfil" }

        ],
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,

        "language": españolTbl
    });
}

//Exportar
//$('#btnExportExcel').on("click", function () {
//    $('#ModalExport').modal('show');
//    $('#1RadioExcel').prop('checked', true);
//    var cont = ConteoActual + 1;
//    for (var i = cont; i <= 3; i++){
        
//        $('#'+i + 'RadioExcel').prop("disabled",true);
//    }
//});

//function habilitarRadioExcel() {
//    $('#1RadioExcel').prop("disabled", false);
//    $('#2RadioExcel').prop("disabled", false);
//    $('#3RadioExcel').prop("disabled", false);
    
//}
//$('#ModalExport').on('show.bs.modal', function () {
//    ////Console.log('se abre el modal');
    
//    ////Console.log('exportConteo:');
//    ////Console.log(ConteoActual);
//    var cont = ConteoActual + 1;
//    for (var i = cont; i <= 3; i++) {
//        ////Console.log('exportConteo:');
//        ////Console.log(i);
//        $('#' + i + 'RadioExcel').prop("disabled", true);
//    }
//});



function CreateExcel(jsonData, SlcConteo) {
    var data = JSON.parse(jsonData);
    ////Console.log(data);
    // Crear el libro y la hoja de cálculo
    var workbook = new ExcelJS.Workbook();
    var worksheet = workbook.addWorksheet('Hoja1');
    // Verificar si hay al menos un objeto en el arreglo
    if (data.length > 0) {
        // Obtener las propiedades del primer objeto como cabeceras
        var headers = Object.keys(data[0]);

        // Agregar las cabeceras a la hoja de cálculo
        worksheet.addRow(headers);

        // Agregar los datos al archivo Excel
        data.forEach(item => {
            var rowData = [];
            headers.forEach(header => {
                rowData.push(item[header] || '');
            });
            worksheet.addRow(rowData);
        });


        //// Calcular las sumatorias de las columnas
        //var sumRow = headers.map(header => {
        //    if (typeof data[0][header] === 'number') {
        //        return data.reduce((acc, curr) => acc + (curr[header] || 0), 0);
        //    } else {
        //        return '';
        //    }
        //});

        //// Agregar la fila de sumatorias al final del archivo Excel
        //worksheet.addRow(sumRow);


        // Ajustar el ancho de las columnas al contenido
        var IndexCol = 0;
        //var GreenCol = true;
        worksheet.columns.forEach((column) => {
            var maxLength = 0;

            column.eachCell({ includeEmpty: true }, (cell) => {
                var columnLength = cell.value ? cell.value.toString().length : 10;
                maxLength = Math.max(maxLength, columnLength);
                //if (IndexCol >= 5 && cell.value.toString().length >= 1) {
                //    //////Console.log(IndexCol);
                //    if (cell.value.toString().charAt(9) == 'E') {
                //        cell.fill = {
                //            type: 'pattern',
                //            pattern: 'solid',
                //            fgColor: {
                //                argb: '92D050'
                //            }
                //        }

                //    } else {

                //        cell.fill = {
                //            type: 'pattern',
                //            pattern: 'solid',
                //            fgColor: {
                //                argb: 'C00000'
                //                //92D050
                //            }
                //        }

                //    }

                //}
            });
            column.width = maxLength < 10 ? 10 : maxLength + 2;


            IndexCol = IndexCol + 1;
            if (IndexCol >= 5 && IndexCol % 2 == 0) { GreenCol = false; } else { GreenCol = true; }
        });

        var IndexRow = 0;
        // Configurar el estilo de las celdas
        worksheet.eachRow({ includeEmpty: true }, (row) => {
            row.eachCell({ includeEmpty: true }, (cell) => {
                cell.border = {
                    top: { style: 'thin' },
                    left: { style: 'thin' },
                    bottom: { style: 'thin' },
                    right: { style: 'thin' }
                };
                /*cell.alignment = { wrapText: true };*/
            });
            if (IndexRow == 0) {
                row.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: {
                        argb: 'FFFFFF'
                    }
                }
            }
            IndexRow = IndexRow + 1;
        });

        // Generar el archivo Excel
        workbook.xlsx.writeBuffer().then((buffer) => {
            var blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var url = URL.createObjectURL(blob);

            // Crear un enlace para descargar el archivo Excel
            var a = document.createElement('a');
            a.href = url;
            a.download = '' + $('#lblInventario').text() +'- Conteo' + SlcConteo +'.xlsx';
            document.body.appendChild(a);
            a.click();

            // Liberar el objeto URL para liberar memoria
            URL.revokeObjectURL(url);
        });
    }
}

//Cargar Graficos//////////////////
var SwitchBarChart = $("#ChartSwitch");
SwitchBarChart.change(function () {
    if (!this.checked) {
        $("#colChart1").css('display', 'block');
        $("#colChart2").css('display', 'none');
        
    } else {
        $("#colChart1").css('display', 'none');
        $("#colChart2").css('display', 'block');
    }

});

//function CargarCuadrosGraficos(codinventario, Inventariados, Faltantes, Total) {
//    $('#dataSquare1').text(codinventario);
//    $('#dataSquare2').text(Inventariados);
//    $('#dataSquare3').text(Faltantes);
//    $('#dataSquare4').text(Total);
//}
function DrawnBarChart1(total) {
    var _url = 'DrawnBarChart';
    var obj = new Object();
    obj.CodInventario = Global_CodInventario;
    obj.tipoGrafico = 0;
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        //async: false,
        success: function (response) {
            var json = JSON.parse(response);

            ////Console.log(json);
            if (json.length > 0) {
                var labels = json.map(function (e) {
                    return e.NOMBRE_USUARIO;
                });
                var data = json.map(function (e) {
                    return e.STOCK_INVENTARIADO;
                });
            }
            DrawnChart1(labels, data,total);
        },
        complete: function (response) {
        }
    });
}

function DrawnChart1(labels,data,Total) {
    var ctx = document.getElementById("Chart1").getContext("2d");

    if (typeof barChart !== 'undefined') {
        barChart.destroy();
    }
    barChart =new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderWidth: 2,
                backgroundColor: [
                    'rgb(13 202 240/ 70%)',
                    'rgb(255 193 7/  70%)',
                    'rgb(220 53 69/  70%)',
                    'rgb(255 193 7/  70%)',
                ],
                //borderColor: [
                //    'rgb(13 202 240)',
                //    'rgb(255 193 7)',
                //    'rgb(220 53 69)',
                //    'rgb(255 193 7)',
                //],
                datalabels: {
                    formatter: (value, ctx) => {
                        let sum = 0;
                        let dataArr = ctx.chart.data.datasets[0].data;
                        dataArr.map(data => {
                            sum += data;
                        });
                        let percentage = (value * 100 / Total).toFixed(2) + "%";
                        return percentage;
                    },
                    color: 'white',
                    backgroundColor: 'rgba(0,0,0,0.5)',
                    borderRadius: 3,
                    font: {
                        weight: 'bold'
                    }
                },
                categoryPercentage: 1.0,
                barPercentage: 0.4
            }]
        },
        plugins: [ChartDataLabels],
        options: {
          
            plugins: {
                legend: {
                    display: false
                },
                title: {
                    display: true,
                    text: 'Lecturas por Usuarios',
                    font: {
                        size: 16,
                        weight: 'bold'
                    },
                    padding: {
                        bottom: 20
                    }
                }
            },
            //scales: {
            //    x: {
            //        grid: {
            //            offset: true
            //        }
            //    }
            //}
        }
    });


}

function DrawnBarChart2(total) {
    var _url = 'DrawnBarChart';
    var obj = new Object();
    obj.CodInventario = Global_CodInventario;
    obj.tipoGrafico = 1;
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        //async: false,
        success: function (response) {
            var json = JSON.parse(response);

            ////Console.log(json);
            if (json.length > 0) {
                var labels = json.map(function (e) {
                    return e.NOMBRE_USUARIO;
                });
                var data = json.map(function (e) {
                    return e.STOCK_INVENTARIADO;
                });
            }
            DrawnChart2(labels, data, total);
        },
        complete: function (response) {
        }
    });

}
function DrawnChart2(labels, data, Total) {
    var ctx = document.getElementById("Chart2").getContext("2d");

    if (typeof barChart2 !== 'undefined') {
        barChart2.destroy();
    }
    barChart2 = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                borderWidth: 2,
                backgroundColor: [
                    'rgb(13 202 240/ 70%)',
                    'rgb(255 193 7/  70%)',
                    'rgb(220 53 69/  70%)',
                    'rgb(255 193 7/  70%)',
                ],
                //borderColor: [
                //    'rgb(13 202 240)',
                //    'rgb(255 193 7)',
                //    'rgb(220 53 69)',
                //    'rgb(255 193 7)',
                //],
                datalabels: {
                    formatter: (value, ctx) => {
                        let sum = 0;
                        let dataArr = ctx.chart.data.datasets[0].data;
                        dataArr.map(data => {
                            sum += data;
                        });
                        let percentage = "Lecturas: " + value;
                        return percentage;
                    },
                    color: 'white',
                    backgroundColor: 'rgba(0,0,0,0.5)',
                    borderRadius: 3,
                    font: {
                        weight: 'bold'
                    }
                },
                categoryPercentage: 1.0,
                barPercentage: 0.4
            }]
        },
        plugins: [ChartDataLabels],
        options: {
           
            plugins: {
                legend: {
                    display: false
                },
                title: {
                    display: true,
                    text: 'Número de lecturas por usuarios',
                    font: {
                        size: 16,
                        weight: 'bold'
                    },
                    padding: {
                        bottom: 20
                    }
                }
            },
            //scales: {
            //    x: {
            //        grid: {
            //            offset: true
            //        }
            //    }
            //}
        }
    });


}

function DrawnChart3(faltantes, inventariado) {
    var ctx = document.getElementById("Chart3").getContext("2d");
    //console.log(faltantes + ' - ' + inventariado);

    if (typeof pieChart !== 'undefined') {
        pieChart.destroy();
    }
    pieChart = new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: ['Faltantes','Inventariados'],
            datasets: [
                {
                    label: 'Inventario',
                    data: [faltantes,inventariado],
                    backgroundColor: [
                        'rgb(220 53 69)',
                        'rgb(255 193 7)',
                    ], hoverOffset: 4,
                    labels: ['Faltantes: ' + faltantes, 'Inventariados: ' + inventariado], 
                    datalabels: {
                        formatter: (value, ctx) => {
                            let suma = 0;
                            let dataArr = ctx.chart.data.datasets[0].data;
                            dataArr.map(data => {
                                suma += parseFloat(data);
                            });
                            
                            //console.log('value: '+value + ' - suma: ' + suma);
                            let percentage = (value * 100 / suma).toFixed(2) + "%";
                            

                            return percentage;
                        },
                        color: 'white',
                        backgroundColor: 'rgba(0,0,0,0.7)',
                        borderRadius: 3,
                        font: {
                            weight: 'bold'
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: (context) => {
                                const label = context.label;
                                const value = context.formattedValue;
                                return `${value}`;
                            },
                        },
                    },
                }
            ]
        },
        plugins: [ChartDataLabels],
        options: {
          
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'AVANCE DE INVENTARIO'
                }
            }          
        }
    });
}


//reportes///////////////////////
/*OcultarFiltros();*/

var RadioFiltroReport = $('input[type="radio"][name="listGroupReport"]');
RadioFiltroReport.change(function () {
    OcultarFiltros();    
});

function OcultarFiltros() {
    //////Console.log('dentro de la funcion');
    $('#filtrosdif').css('display', 'none');
    $('#filtrosProd').css('display', 'none');
    $('#filtrosUbic').css('display', 'none');
    $('#filtrosLectxUsuario').css('display', 'none');

    var radSelect = $('input:radio[name=listGroupReport]:checked').val();
    switch (radSelect) {
        case "0": $('#filtrosdif').css('display', 'block'); break;
        case "1": $('#filtrosProd').css('display', 'block'); break;
        case "2": $('#filtrosUbic').css('display', 'block');  break;
        case "3": $('#filtrosLectxUsuario').css('display', 'block'); break;
        default: ////Console.log('Sin selección de reporte');; break;
    }
    $('#RangeReport1').prop("max", ConteoActual.toString());
    $('#RangeReport3').prop("max", ConteoActual.toString());
    cargarRange();

}
function cargarRange() {
    $("#rangeMarker").empty();
    $("#rangeMarker3").empty();
    var row = "";
    for (var i = 1; i <= ConteoActual; i++) {
        row += "<option value="+i+" label="+i+">"+i+"</option>" ;
    }
    $("#rangeMarker").html(row);
    $("#rangeMarker3").html(row);
}

$("#Gestor_btnGenerarReporte").on("click", function () {
    GenerarReporte();
});
function GenerarReporte() {

    var radSelect = $('input:radio[name=listGroupReport]:checked').val();

    var _url = 'ReportesInventario_WEB';
    var obj = new Object();

    obj.tipoReporte = radSelect;
    obj.Cod_inventario = $('#lblInventario').text();
    switch (radSelect) {
        case "0": obj.DatoFiltro = $('#RangeReport1').val(); break;
        case "1": obj.DatoFiltro = $('#txtCodProduct').val(); break;
        case "2": obj.DatoFiltro = $('#txtCodUbicacion').val();
            obj.NConteo = $('#RangeReport3').val();
            obj.Usuario = $('#txtCodUsuario1').val();
            break;
        case "3": obj.DatoFiltro = $('#txtCodUsuario2').val(); break;
        default: ////Console.log('Sin selección de reporte');; break;
    }

    if (radSelect!=2) {obj.NConteo = ConteoActual;}
    if (radSelect != 2) { obj.Usuario = ""; }
    
    OcultarTablasReport();
    switch (obj.tipoReporte) {
        case '0': GenerarRe_Diferencial(_url, obj, obj.DatoFiltro); break;
        case '1': GenerarRe_Productos(_url,obj); break;
        case '2': GenerarRe_Ubicaciones(_url,obj); break;
        case '3': GenerarRe_LecturaxUsuario(_url,obj); break;
        default: ////Console.log('Sin selección de reporte');; break;
    }

    //$.ajax({
    //    url: _url,
    //    type: 'POST',
    //    contentType: "application/json; charset=utf-8",
    //    data: JSON.stringify(obj),
    //    //async: false,
    //    success: function (response) {
    //        OcultarTablasReport();
    //        var json = JSON.parse(response);
    //        ////Console.log(json);
    //        switch (obj.tipoReporte) {
    //            case '0': GenerarRe_Diferencial(json, obj.DatoFiltro); break;
    //            case '1': GenerarRe_Productos(json); break;
    //            case '2': GenerarRe_Ubicaciones(json); break;
    //            case '3': GenerarRe_LecturaxUsuario(json); break;
    //            default: ////Console.log('Sin selección de reporte');; break;
    //        }

    //    },
    //    complete: function (response) {
    //    }
    //});

}
function GenerarRe_Diferencial(_url,data, conteo) {
    if (typeof Report1_Tbl !== 'undefined') {
        Report1_Tbl.destroy();
        
    }
    $('#contenedorTblDiv1').css('display', 'block');
    var conteoreporte;
    if (conteo == "1") { conteoreporte = 'CONTEO_UNO'; }
    if (conteo == "2") { conteoreporte = 'CONTEO_DOS'; }
    if (conteo == "3") { conteoreporte = 'CONTEO_TRE'; }

    

    Report1_Tbl = $('#Tbl_report1').DataTable({
        "serverSide": true,
        "ajax": {
            "url": _url, 
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.tipoReporte = data.tipoReporte;
                f.Cod_inventario = data.Cod_inventario;
                f.DatoFiltro = data.DatoFiltro;
                f.NConteo = data.NConteo;
                f.Usuario = data.Usuario;
                f.searchValue = $('input[type="search"][aria-controls="Tbl_report1"]').val();
                //f.search
                ////Console.log(f);
                toggle_Loadingtb('tbLoading', true);
            }, "complete": function (response) {

                $('#Report1Ini').text(response.responseJSON.Listfooter[0]);
                $('#Report1Fin').text(response.responseJSON.Listfooter[1]);
                $('#Report1Dif').text(response.responseJSON.Listfooter[2]);

                toggle_Loadingtb('tbLoading', false);

            }
        },

        "createdRow": function (row, data, dataIndex) {
            
            if (data.STOCK_DIFERENCIAL === "0") {
                ////Console.log('pinta verde');
                $(row).addClass('rowGreen');
                //$(row).find('.btnDelete').prop('disabled', true);

            }
        },
        "columns": [
            { "data": "COD_PRODUCTO", "title": "COD.PRODUCTO" },
            { "data": "DSC_PRODUCTO", "title": "PRODUCTO" },
            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA
            { "data": "DSC_UBICACION", "title": "UBICACIÓN" },
            { "data": "LOTE_PRODUCTO", "title": "LOTE_PRODUCTO" },
            //{ "data": "SERIE_PRODUCTO", "title": "SERIE_PRODUCTO" },
            { "data": "STOCK_INICIAL", "title": "CANTIDAD_INICIAL" },
            { "data": conteoreporte, "title": "INVENTARIADO" },
            { "data": "STOCK_DIFERENCIAL", "title": "DIFERENCIAL" }

        ],
        "footerCallback": function (row, data, start, end, display) {

            var api = this.api();
            //////Console.log(api);
            //var TStockInicial = api.column(5).data().sum();
            //var TConteo1 = api.column(6).data().sum();
            //var TConteo2 = api.column(7).data().sum();
            //var TConteo3 = api.column(8).data().sum();
            //var TStockFinal = api.column(9).data().sum();
            //var TStockDiferencial = api.column(10).data().sum();

            //$('#Report1Ini').text(api.column(4).data().sum());
            //$('#Report1Fin').text(api.column(5).data().sum());
            //$('#Report1Dif').text(api.column(6).data().sum());

        },

        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,

        "language": españolTbl
    });


}
function GenerarRe_Productos(_url,data) {
    
    if (typeof Report2_Tbl !== 'undefined') {
        Report2_Tbl.destroy();
    }
    $('#contenedorTblDiv2').css('display', 'block');

    Report2_Tbl = $('#Tbl_report2').DataTable({
        "serverSide": true,
        "ajax": {
            "url": _url,
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.tipoReporte = data.tipoReporte;
                f.Cod_inventario = data.Cod_inventario;
                f.DatoFiltro = data.DatoFiltro;
                f.NConteo = data.NConteo;
                f.Usuario = data.Usuario;
                f.searchValue = $('input[type="search"][aria-controls="Tbl_report2"]').val();
                //f.search
                //////Console.log(f);
                toggle_Loadingtb('tbLoading', true);
            }, "complete": function (response) {
                $('#Report2Cant').text(response.responseJSON.Listfooter[0]);
                toggle_Loadingtb('tbLoading', false);
            }
        },

        
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "DSC_PRODUCTO", "title": "DESC.PRODUCTO" },
            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA
            { "data": "COD_UBICACION", "title": "COD.UBICACIÓN" },
            { "data": "LOTE_PRODUCTO", "title": "LOTE_PRODUCTO" },
            //{ "data": "SERIE_PRODUCTO", "title": "SERIE_PRODUCTO" },
            { "data": "STOCK_INVENTARIADO", "title": "CANT. INVENTARIADA" },
            { "data": "COD_USUARIO_REGISTRO", "title": "COD_USUARIO" },

        ],
        "footerCallback": function (row, data, start, end, display) {

            //var api = this.api();
            //$('#Report2Cant').text(api.column(2).data().sum());
        },

        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,

        "language": españolTbl
    });

}
function GenerarRe_Ubicaciones(_url,data) {
    if (typeof Report3_Tbl !== 'undefined') {
        Report3_Tbl.destroy();
    }
    $('#contenedorTblDiv3').css('display', 'block');

    Report3_Tbl = $('#Tbl_report3').DataTable({
        "serverSide": true,
        "ajax": {
            "url": _url,
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.tipoReporte = data.tipoReporte;
                f.Cod_inventario = data.Cod_inventario;
                f.DatoFiltro = data.DatoFiltro;
                f.NConteo = data.NConteo;
                f.Usuario = data.Usuario;
                f.searchValue = $('input[type="search"][aria-controls="Tbl_report3"]').val();
                //f.search
                //////Console.log(f);
                toggle_Loadingtb('tbLoading', true);
            }, "complete": function (response) {
                $('#Report3Cant').text(response.responseJSON.Listfooter[0]);
                toggle_Loadingtb('tbLoading', false);
            }
        },

        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "COD_PRODUCTO", "title": "COD.PRODUCTO" },
            { "data": "DSC_PRODUCTO", "title": "PRODUCTO" },
            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA
            { "data": "LOTE_PRODUCTO", "title": "LOTE_PRODUCTO" },
            //{ "data": "SERIE_PRODUCTO", "title": "SERIE_PRODUCTO" },
            { "data": "STOCK_INVENTARIADO", "title": "CANTIDAD" },
            { "data": "COD_USUARIO_REGISTRO", "title": "COD_USUARIO" },
        ],
        "footerCallback": function (row, data, start, end, display) {

            var api = this.api();
          /*  $('#Report3Cant').text(api.column(3).data().sum());*/
        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,

        "language": españolTbl
    });

}
function GenerarRe_LecturaxUsuario(_url,data) {
    if (typeof Report4_Tbl !== 'undefined') {
        Report4_Tbl.destroy();
    }
    $('#contenedorTblDiv4').css('display', 'block');

    Report4_Tbl = $('#Tbl_report4').DataTable({
        "serverSide": true,
        "ajax": {
            "url": _url,
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.tipoReporte = data.tipoReporte;
                f.Cod_inventario = data.Cod_inventario;
                f.DatoFiltro = data.DatoFiltro;
                f.NConteo = data.NConteo;
                f.Usuario = data.Usuario;
                f.searchValue = $('input[type="search"][aria-controls="Tbl_report4"]').val();
                //f.search
                //////Console.log(f);
                toggle_Loadingtb('tbLoading', true);
            }, "complete": function (response) {
                $('#Report4Cant').text(response.responseJSON.Listfooter[0]);

                toggle_Loadingtb('tbLoading', false);
            }
        },
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "COD_PRODUCTO", "title": "COD.PRODUCTO" },
            { "data": "DSC_PRODUCTO", "title": "PRODUCTO" },
            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA
            { "data": "COD_UBICACION", "title": "UBICACIÓN" },
            { "data": "LOTE_PRODUCTO", "title": "LOTE_PRODUCTO" },
            //{ "data": "SERIE_PRODUCTO", "title": "SERIE_PRODUCTO" },
            { "data": "STOCK_INVENTARIADO", "title": "CANTIDAD" }
            
        ],
        "footerCallback": function (row, data, start, end, display) {

            //var api = this.api();
            //$('#Report4Cant').text(api.column(4).data().sum());
        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,

        "language": españolTbl
    });
}
function OcultarTablasReport() {
    ////Console.log('dentro de la funcion');
    $('.divTbl').css('display', 'none');

}



///////////////////////////////CREAR INVENTARIO CON API

$("#API_btn_Add").on("click", function () {
    $("#API_btn_Add").attr("disabled", true);
    toggle_Loadingtb('tbLoading', true);
    CrearInvAPI();
});
function CrearInvAPI() {
    var Cod_Alm = $('#API_cbxAlmacen').val();
    var obj = new Object();

    obj.COD_ALM = Cod_Alm;
    let _url = "CargarAlmacenesAPIExterna";
    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        //async: false,

        success: function (response) {
            
            if (response.HUBO_ERROR) {
                MostrarError(response.MENSAJE_ERROR, "API_content_mensajes");
            }
            else {
                $('#txtvar').text(response.MENSAJE_ERROR);
                $('#success_modal').modal('show');
                $('#success_modal').on('hidden.bs.modal', function () {
                    location.reload();
                });
            }
            $("#API_btn_Add").attr("disabled", false);
        },
        complete: function () {
            $("#Api_Almacen").attr("disabled", false);
            toggle_Loadingtb('tbLoading', false);
                       
        },
        error: function (result) {

            //Console.log('error' + result);
        }
    });

}