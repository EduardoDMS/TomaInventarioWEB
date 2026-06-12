/*// TRAE LOS ALMACENES //*/
cargarcbxInventario();
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
            let option = "";

            if (response.length === 0) {
                option += "<option value='-1'>--Sin resultados--</option>";
            } else {
                $.each(response, function (i, item) {
                    option += "<option value='" + item.intValue + "'>" + item.vchdesc + "</option>";
                });
            }
            $("#cbxInventarioP").html(option);
        },
        error: function (result) {
            Swal.fire("Error inesperado", "Ocurrió un problema al procesar la solicitud. Intente nuevamente.", "error");
        }
    });
}



/*// MOSTRAR INFORMACION AL CAMBIAR DE ALMACEN //*/
const slcAlmacen = document.getElementById("cbxInventarioP");
const contenedorInfo1 = document.getElementById("cntInfo1"); // Numero de inventarios y boton exportar excel
const contenedorInfo2 = document.getElementById("cntInfo2"); // Seccion de filtros y tabla de inventarios

slcAlmacen.addEventListener("change", function () {
    if (slcAlmacen.value === "-1") {
        OcultarInfo();
        return;
    }

    MostrarInfo();
});



function MostrarInfo() {
    CargarHistorico();

    contenedorInfo1.classList.remove("d-none");
    contenedorInfo2.classList.remove("d-none");
}

function OcultarInfo() {
    contenedorInfo1.classList.add("d-none");
    contenedorInfo2.classList.add("d-none");
}



/*// REALIZAR FILTROS POR FECHA //*/
const inputFechaInicio = document.getElementById("dtmIni");
const inputFechaFin = document.getElementById("dtmFin");
let chkfiltro = "0";

inputFechaInicio.addEventListener("change", evaluarFechas);
inputFechaFin.addEventListener("change", evaluarFechas);
function evaluarFechas() {
    const fechaI = inputFechaInicio.value;
    const fechaF = inputFechaFin.value;

    if (fechaI && fechaF) {
        chkfiltro = "1";
        CargarHistorico();
    } else {
        chkfiltro = "0";
        console.log("Debe ingresar ambas fechas");
    }
}



/*// REALIZAR FILTROS POR ESTADO //*/
const slcEstado = document.getElementById("slcEstado");
let estadoSeleccionado = "Todos";

slcEstado.addEventListener("change", function () {
    estadoSeleccionado = slcEstado.value;
    CargarHistorico();
});

function evaluarEstado(data) {
    if (estadoSeleccionado === "Todos") {
        return data;
    }

    return data.filter(item => item.DSC_ESTADO === estadoSeleccionado);
}

/*// REALIZAR FILTROS POR CONTEO //*/
const slcConteo = document.getElementById("slcConteo");
let conteoSeleccionado = "Todos";

if (slcConteo) {
    slcConteo.addEventListener("change", function () {
        conteoSeleccionado = slcConteo.value;
        CargarHistorico();
    });
}

function evaluarConteo(data) {
    if (conteoSeleccionado === "Todos") {
        return data;
    }
    const conteo = parseInt(conteoSeleccionado);

    return data.filter(item => item.NRO_CONTEO === conteo);
}



/*// BOTON LIMPIAR FILTROS //*/
const btnLimpiarFiltro = document.getElementById("btn_limpiar");

btnLimpiarFiltro.addEventListener("click", function () {
    inputFechaInicio.value = "";
    inputFechaFin.value = "";
    chkfiltro = "0";

    slcEstado.value = "Todos";
    estadoSeleccionado = "Todos";

    slcConteo.value = "Todos";
    conteoSeleccionado = "Todos";

    CargarHistorico();
});



/*// MOSTRAR TABLA DE INVENTARIOS - REPORTES DETALLADO Y LECTURA //*/
function CargarHistorico() {
    const obj = {
        ID_Almacen: $('#cbxInventarioP').val(),
        filtrarFecha: chkfiltro,
        dtmIni: $('#dtmIni').val(),
        dtmFin: $('#dtmFin').val()
    };   

    var _url = 'TablaHistorico';
    $.ajax({
        url: _url,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        //async: false,
        success: function (response) {
            
            const json = JSON.parse(response);
            const dataEstado = evaluarEstado(json);
            const dataFinal = evaluarConteo(dataEstado);

            //console.log(dataFinal);

            $('#spanCantInv').text(dataFinal.length);
            mostrarHistorico(dataFinal);
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
            { "data": "COD_INVENTARIO", "title": "Código de Inventario" },
            { "data": "DSC_ALMACEN", "title": "Almacén" },
            { "data": "FCH_INICIO", "title": "Fecha Apertura" },
            { "data": "FCH_FIN", "title": "Fecha Cierre" },
            { "data": "NRO_CONTEO", "title": "Conteo" },
            { "data": "DSC_ESTADO", "title": "Estado" },
            { "data": "DSC_EMPRESA", "title": "Empresa" },
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

    Historico_tabla.on('click', 'tbody tr', function () {
        const data = Historico_tabla.row(this).data();
        //console.log(data);

        codInventarioSelect = data.COD_INVENTARIO;
        nroConteoSelect = data.NRO_CONTEO;
        almacenSelect = data.DSC_ALMACEN;

        if ($(this).hasClass("selected")) {
            $(this).removeClass("selected");

            estaSelectInv = false;
            return;
        }

        Historico_tabla.$("tr.selected").removeClass("selected");
        $(this).addClass("selected");

        estaSelectInv = true;
    });
}
