var init = true;
$('#cbxInventarioP').change(function () {
    var cbxSelect = $('#cbxInventarioP')[0].selectedIndex
    if (cbxSelect == 0) {
        cerrarReporte();
    } else {
        abrirReporte();
    }
})

$("#chkFiltrodtm").change(function () {
    disabledfiltro();
});

function LoadCbxInventario() {
    cargarcbxInventario();
    disabledfiltro();
    var cbxSelect = $('#cbxInventarioP')[0].selectedIndex
    if (cbxSelect == 0) {
        cerrarReporte();
    } else {
        abrirReporte();
    }
}
function cargarcbxInventario() {   
    $("#cbxInventarioP").empty();
    let _url = 'FillCbxInventario';
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: '',
        async: false,
        success: function (response) {
            //console.log(response);
            //  //Console.log(response);
            var row = "";
            /*row += "<option value=''>--Seleccione--</option>";*/
            if (response.length === 0) {
                row += "<option value='-1'>--Sin resultados--</option>";
            } else {
                $.each(response, function (i, item) {
                    row += "<option value='" + item.intValue + "'>" + item.vchdesc + "</option>";
                });
            }
            $("#cbxInventarioP").html(row);
        },
        error: function (result) {
            ////Console.log('error' + result);
        }
    });
}
function disabledfiltro() {
    var chkFiltro = document.getElementById('chkFiltrodtm').checked;
    ////Console.log("marcado?= "+chkFiltro);
    if (chkFiltro) {
        $('#dtmIni').prop('disabled', false);
        $('#dtmFin').prop('disabled', false);
    } else {
        $('#dtmIni').prop('disabled', true);
        $('#dtmFin').prop('disabled', true);
    }
}
function abrirReporte() {
    $('#GestorPrincipal').removeClass('d-none');
    $('#lblAlmacen').text($('#cbxInventarioP option:selected').text());
    CargarHistorico()

}
function cerrarReporte() {
    $('#GestorPrincipal').addClass('d-none');
}

function abrirReporte() {
    $('#GestorPrincipal').removeClass('d-none');
    $('#cardInfoInicial').addClass('d-none'); // Ocultar card informativo
    $('#lblAlmacen').text($('#cbxInventarioP option:selected').text());
    limpiarFiltroFechas();

    CargarHistorico()
}

// Nueva función para limpiar el filtro de fechas
function limpiarFiltroFechas() {
    // Desmarcar el checkbox
    $('#chkFiltrodtm').prop('checked', false);

    // Limpiar los valores de las fechas
    $('#dtmIni').val('');
    $('#dtmFin').val('');

    // Deshabilitar los campos de fecha
    $('#dtmIni').prop('disabled', true);
    $('#dtmFin').prop('disabled', true);
}

function limpiarFiltro() {
    // Desmarcar el checkbox
    $('#chkFiltrodtm').prop('checked', false);

    // Limpiar los valores de las fechas
    $('#dtmIni').val('');
    $('#dtmFin').val('');

    // Recargar la tabla
    reloadTableHistorico();
}

// Evento para el botón limpiar
$('#btn_limpiar').on('click', function () {
    limpiarFiltro();
});

// Evento para el botón buscar
//$('#btnBuscar').on('click', function () {
//    reloadTableHistorico();
//});


function cerrarReporte() {
    $('#GestorPrincipal').addClass('d-none');
    $('#cardInfoInicial').removeClass('d-none'); // Mostrar card informativo nuevamente
}
function destroyTableHistorico() {
    if (!init) {
        $('#tbl_Historico').DataTable().destroy();
    }
    init = false;
}

function CargarHistorico() {
    var obj = new Object();
    obj.ID_Almacen = $('#cbxInventarioP').val();
    var chkfiltro = "0"
    if (document.getElementById('chkFiltrodtm').checked) { chkfiltro = "1" }
    obj.filtrarFecha = chkfiltro;
    obj.dtmIni = $('#dtmIni').val();
    obj.dtmFin = $('#dtmFin').val();

    var _url = 'TablaHistorico';
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        //async: false,
        success: function (response) {
            console.log(response);
            var json = JSON.parse(response);
            ////Console.log(json.length);
            $('#spanCantInv').text(json.length);
            mostrarHistorico(json);

        },
        complete: function (response) {
        }
    });



}
var Historico_tabla;
function mostrarHistorico(data) {

    if (typeof Historico_tabla !== 'undefined') {
        ////Console.log('destruir');

        Historico_tabla.on('click', 'tbody tr', (e) => {
            ////Console.log('ccc');
            let clsList = e.currentTarget.classList;

            if (clsList.contains('selected')) {
                ////Console.log('cc1');
                clsList.remove('selected');
            }
            else {
                ////Console.log('cc2');
                Historico_tabla.rows('.selected').nodes().each((row) => row.classList.remove('selected'));
                clsList.add('selected');
            }
        });
        Historico_tabla.destroy();
    }
    Historico_tabla = $('#tbl_Historico').DataTable({
        "data": data,
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "COD_INVENTARIO", "title": "CÓDIGO INVENTARIADO" },
            { "data": "DSC_ALMACEN", "title": "ALMACÉN" },
            { "data": "FCH_INICIO", "title": "FECHA INICIO" },
            { "data": "FCH_FIN", "title": "FECHA FIN" },
            { "data": "NRO_CONTEO", "title": "CONTEO" },
            { "data": "DSC_ESTADO", "title": "ESTADO" },
            { "data": "DSC_EMPRESA", "title": "EMPRESA" },
        ],
        "footerCallback": function (row, data, start, end, display) {

        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,
        /*"select": true,*/
        "language": españolTbl
    });

    Historico_tabla.on('click', 'tbody tr', (e) => {
        ////Console.log('aaaaaa');
        let classList = e.currentTarget.classList;

        if (classList.contains('selected')) {
            ////Console.log('bbbb1');
            classList.remove('selected');
        }
        else {
            ////Console.log('bbbb2');
            Historico_tabla.rows('.selected').nodes().each((row) => row.classList.remove('selected'));
            classList.add('selected');
        }
    });

    //boton DETALLES DE INVENTARIO
    document.querySelector('#btnDetalle').addEventListener('click', function () {
        var filaSelect = Historico_tabla.row('.selected').data();
        //Console.log(filaSelect);
        if (filaSelect) {
            var cod_Inventario = filaSelect.COD_INVENTARIO;
            var NConteo = filaSelect.NRO_CONTEO;
            $('#ModalDetallado').modal('show');
            CargarDetalle(cod_Inventario, NConteo);
            $('#txtEmpresa_det').val(filaSelect.DSC_EMPRESA);
            $('#txtAlmacen_det').val(filaSelect.DSC_ALMACEN);
            $('#txtConteo_det').val("Conteo " + filaSelect.NRO_CONTEO);
            $('#txtCodInv_det').val(filaSelect.COD_INVENTARIO);
            $('#txtEstadoInv_det').val(filaSelect.DSC_ESTADO);

        } else {
            //$('#txtvarError').text("No se ha seleccionado una fila");
            //$('#error_modal').modal('show');
            // Alerta cuando no hay fila seleccionada
            Swal.fire({
                icon: 'info',
                title: 'Información',
                text: 'Selecciona una fila de la tabla para ver los detalles',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Aceptar',
                //timer: 2500,
                //timerProgressBar: true

            });


        }
    });

    //boton Lecturas
    document.querySelector('#btnLecturas').addEventListener('click', function () {
        var filaSelect = Historico_tabla.row('.selected').data();
        if (filaSelect) {
            var cod_Inventario = filaSelect.COD_INVENTARIO;
            var NConteo = filaSelect.NRO_CONTEO;
            $('#ModalLecturas').modal('show');
            CargarLecturas(cod_Inventario, NConteo);
            $('#txtEmpresa_lec').val(filaSelect.DSC_EMPRESA);
            $('#txtAlmacen_lec').val(filaSelect.DSC_ALMACEN);
            $('#txtCodInv_lec').val(cod_Inventario);
            $('#txtEstInv_lec').val(filaSelect.DSC_ESTADO);

        } else {
            //$('#txtvarError').text("No se ha seleccionado una fila");
            //$('#error_modal').modal('show');
            Swal.fire({
                icon: 'info',
                title: 'Información',
                text: 'Selecciona una fila de la tabla para ver las lecturas',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Aceptar',
                //timer: 2500,
                //timerProgressBar: true

            });

        }
    });


}

document.getElementById("btnBuscar").onclick = function () {
    reloadTableHistorico();
};
document.getElementById("actLecturas").onclick = function () {
    reloadTableLecturas()
};


function reloadTableHistorico() {
    setTimeout(function () {
        //$('#tbl_Historico').DataTable().ajax.reload(null, true);
        CargarHistorico();
    }, 500);
}
function reloadTableLecturas() {
    setTimeout(function () {
        var dtLecCodInv = $('#datosLecturas').attr('codInv');
        var dtLecNCont = $('#datosLecturas').attr('NConteo');

        CargarLecturas(dtLecCodInv, parseInt(dtLecNCont, 10))
    }, 500);
}
function CargarDetalle(cod_Inventario, NConteo) {
    $('#datosDetallado').attr('codInv', cod_Inventario);
    $('#datosDetallado').attr('NConteo', NConteo);

    var obj = new Object();
    obj.cod_Inventario = cod_Inventario;
    obj.NConteo = NConteo;
    var _url = 'TablaDetalle_WEB';


    mostrarDetalle(_url, obj);
    //$.ajax({
    //    url: _url,
    //    type: 'POST',
    //    contentType: "application/json; charset=utf-8",
    //    data: JSON.stringify(obj),
    //    //async: false,
    //    success: function (response) {
    //        var json = JSON.parse(response);
    //        ////Console.log(json.length);
    //        ////Console.log(json);
    //        mostrarDetalle(json);
    //    },
    //    complete: function (response) {
    //    }
    //});
}

function mostrarDetalle(_url, data) {
    if (typeof Detalle_tabla !== 'undefined') {
        ////Console.log('destruir');
        Detalle_tabla.destroy();
    }
    Detalle_tabla = $('#Detalle_Tbl').DataTable({
        //"data": data,
        "serverSide": true,
        "ajax": {
            "url": _url,
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.cod_Inventario = data.cod_Inventario;
                f.NConteo = data.NConteo;
                f.searchValue = $('input[type="search"][aria-controls="Detalle_Tbl"]').val();
                //f.search
                //Console.log(f);
                Swal.fire({
                    title: 'Cargando...',
                    html: 'Por favor espere',
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    showConfirmButton: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
            }, "complete": function (response) {
                Swal.close();
            }
        },
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "COD_UBICACION", "title": "COD UBICACIÓN" },
            //{ "data": "DSC_UBICACION", "title": "UBICACIÓN" },
            { "data": "COD_PRODUCTO", "title": "COD PRODUCTO" },
            { "data": "DSC_PRODUCTO", "title": "PRODUCTO" },
            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA

            { "data": "LOTE_PRODUCTO", "title": "LOTE" },
            { "data": "STOCK_INICIAL", "title": "STOCK" },
        ],
        "footerCallback": function (row, data, start, end, display) {

        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,
        /*"select": true,*/
        "language": españolTbl
    });

}

function CargarLecturas(cod_Inventario, NConteo) {
    $('#datosLecturas').attr('codInv', cod_Inventario);
    $('#datosLecturas').attr('NConteo', NConteo);

    var obj = new Object();
    obj.cod_Inventario = cod_Inventario;
    obj.NConteo = NConteo;


    var _url = 'TablaLecturas';
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        //async: false,
        success: function (response) {
            var json = JSON.parse(response);
            ////Console.log(json.length);
            //Console.log(json);
            mostrarLecturas(json);
        },
        complete: function (response) {
        }
    });
}

function mostrarLecturas(data) {
    if (typeof Lecturas_tabla !== 'undefined') {
        ////Console.log('destruir');
        Lecturas_tabla.destroy();
    }
    Lecturas_tabla = $('#Lecturas_Tbl').DataTable({
        "data": data,
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "FCH_REGISTRO_TERMINAL", "title": "FECHA REGISTRO" },
            { "data": "COD_PRODUCTO", "title": "COD. PRODUCTO" },
            { "data": "DSC_PRODUCTO", "title": "PRODUCTO" },

            //IQFARMA
            //{ "data": "DSC_ANALISIS", "title": "ANALISIS" },
            //{ "data": "DSC_OBSERVACION", "title": "OBSERVACION" },
            //{ "data": "DSC_BALANZA", "title": "BALANZA" },
            //FIN IQFARMA

            { "data": "LOTE_PRODUCTO", "title": "LOTE" },
            //{ "data": "SERIE_PRODUCTO", "title": "SERIE" },
            { "data": "STOCK_INVENTARIADO", "title": "CANTIDAD" },
            { "data": "COD_USUARIO_REGISTRO", "title": "USUARIO" },
            { "data": "ID_TERMINAL", "title": "TERMINAL" },
            { "data": "DSC_UBICACION", "title": "UBICACIÓN" },
        ],
        "footerCallback": function (row, data, start, end, display) {

        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": false,
        /*"select": true,*/
        "language": españolTbl
    });

}