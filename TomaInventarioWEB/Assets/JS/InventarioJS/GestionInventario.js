let Global_CodInventario = "";
let ConteoActual = 0;
let Codigo_inventario = "";
let Codigo_almacen = "";
let EstadoInventario = "";
let Interval = false;
let nIntervId;


/*// TRAE LOS INVENTARIOS //*/
LoadCbxInventario();
function LoadCbxInventario() {
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

            let opcion = "";
            if (response.length === 0) {
                opcion += "<option value='-1'>--No se encuentran inventarios--</option>";
            } else {
                $.each(response, function (i, item) {
                    opcion += "<option value='" + item.vchValue + "'>" + item.vchdesc + "</option>";
                });
            }

            $("#cbxInventarioP").html(opcion);
        },
        error: function (result) {
            Swal.fire("Error inesperado", "Ocurrió un problema al procesar la solicitud. Intente nuevamente.", "error");
        }
    });
}


/*// MUESTRA INFO DE INVENTARIO SELECCIONADO //*/
function MostrarGestion(Cod_Inventario) {    
    contenidoGestion.classList.remove("d-none");
    contenidoTabs.classList.remove("d-none");
    contenidoInformacion.classList.remove("d-none");

    //Actualizar codigo de inventario
    Global_CodInventario = Cod_Inventario;

    //Mostrar tabla
    CargarDatosInventario(Cod_Inventario);
    CargarTablaInventario(Cod_Inventario);  

    //Iniciar recarga de tabla
    Interval = true;
    cargarInterval();
}


/*// OCULTA INFO SI NO HAY SELECCION //*/
function OcultarGestion() {
    contenidoGestion.classList.add("d-none");
    contenidoTabs.classList.add("d-none");
    contenidoInformacion.classList.add("d-none");
}


/*// TRAE LOS DATOS DEL INVENTARIO //*/
function CargarDatosInventario(Cod_Inventario) {
    let _url = 'GetInventario';
    var obj = new Object();
    obj.idEmpresa = '1';
    obj.Almacen = -1;
    obj.CodInventario = Cod_Inventario;
    obj.CodEstado = 'A';
    obj.flg_filtroFecha = '0';
    obj.fch_inicio = '';
    obj.fch_fin = '';
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        async: false,
        success: function (response) {
            //console.log(response);
            ConteoActual = response[0].NRO_CONTEO;
            EstadoInventario = response[0].DSC_ESTADO;
            lblConteoActual.textContent = ConteoActual;
        },
        error: function (result) {
        }
    });
}


/*// TOGGLE MOSTRAR/OCULTAR COLUMNAS DE LOTE //*/
$('#chkOcultar').on("change", function () {
    aplicarVisibilidadLotes();
});


function aplicarVisibilidadLotes() {
    if (typeof Inventario_tabla === 'undefined' || !Inventario_tabla) return;

    var mostrar = !document.getElementById('chkOcultar').checked;

    Inventario_tabla.column('col_lote_inicial:name').visible(mostrar);
    Inventario_tabla.column('col_lote_contado:name').visible(mostrar);

    aplicarEstiloConteoActual();
}


function aplicarEstiloConteoActual() {

    if (typeof Inventario_tabla === 'undefined' || !Inventario_tabla) return;

    const columnasConteo = [
        'Head_Cont1:name',
        'Head_Cont2:name',
        'Head_Cont3:name'
    ];

    // Quitar estilos de las columnas
    Inventario_tabla
        .columns(columnasConteo)
        .nodes()
        .to$()
        .removeClass('fw-bold text-primary fuente-conteo-actual');

    // Quitar estilos de las cabeceras
    $(Inventario_tabla.columns(columnasConteo).header())
        .removeClass('text-primary cabecera-conteo-actual');

    let columna = null;

    switch (ConteoActual) {

        case 1:
            columna = 'Head_Cont1:name';
            break;

        case 2:
            columna = 'Head_Cont2:name';
            break;

        case 3:
            columna = 'Head_Cont3:name';
            break;
    }

    if (!columna) return;

    // CABECERA
    $(Inventario_tabla.column(columna).header())
        .addClass('text-primary cabecera-conteo-actual');

    // TODA LA COLUMNA
    Inventario_tabla
        .column(columna)
        .nodes()
        .to$()
        .addClass('fw-bold text-primary fuente-conteo-actual');
}


/*// CREAR TABLA DE INVENTARIO //*/
function CargarTablaInventario(Cod_Inventario) {

    if (typeof Inventario_tabla !== 'undefined') {
        Inventario_tabla.destroy();
    }

    Inventario_tabla = $('#tbl_Inventario').DataTable({
       
        "serverSide": true,
        "ajax": {
            "url": "ListarInventario",
            "type": "POST",
            "datatype": "json",
            "data": function (f) {
                f.COD_INVENTARIO = Cod_Inventario;
                f.NRO_CONTEO_1 = 0;
                f.NRO_CONTEO_2 = 0;
                f.NRO_CONTEO_3 = 0;
                f.searchValue = $('input[type="search"][aria-controls="tbl_Inventario"]').val();
            }, "complete": function (response) {
                //console.log(response.responseJSON);

                var codAlmacen = response.responseJSON.data[0].Dsc_Almacen;
                $('#lblAlmAct').text(`Almacen actual: ${codAlmacen}`);
                $('#lblInvAct').text(`Inventario actual: ${Cod_Inventario}`);

                Codigo_inventario = Cod_Inventario;
                Codigo_almacen = codAlmacen;

                let data = response.responseJSON.data

                var totalElementos = response.responseJSON.recordsTotal;
                var InventarioContado = response.responseJSON.Listfooter[6];
                $('#tblConteoData').text(InventarioContado + '/' + totalElementos);

                var faltantes = totalElementos - InventarioContado;

                CargarCuadrosGraficos(Cod_Inventario, InventarioContado, faltantes, totalElementos);
                
                CargarGraficoDiferencial(data);

                CargarFooter(response.responseJSON.Listfooter[0], response.responseJSON.Listfooter[1], response.responseJSON.Listfooter[2], response.responseJSON.Listfooter[3],
                    response.responseJSON.Listfooter[4], response.responseJSON.Listfooter[5]);
                var TStockInicial = $('#FCantI').text();
                var TConteo1 = $('#FCont1').text();
                var TConteo2 = $('#FCont2').text();
                var TConteo3 = $('#FCont3').text();

                var Inventariado = 0;
                if (ConteoActual == 1) { Inventariado = TConteo1; }
                if (ConteoActual == 2) { Inventariado = TConteo2; }
                if (ConteoActual == 3) { Inventariado = TConteo3; }


                var Faltantes = TStockInicial - Inventariado;

                GraficoStock(Inventariado, Faltantes);

                DrawnBarChart1(TStockInicial);
                
                //DrawnBarChart2(TStockInicial);
                //DrawnChart3(Faltantes, Inventariado);
            }
        },
        "drawCallback": function () {
            const tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );

            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        },
        "createdRow": function (row, data, dataIndex, cells) {           

            // Registro nuevo
            if (data.FLG_ESNUEVO === '1') {
                row.classList.add("row-nuevo");

                row.setAttribute("data-bs-toggle", "tooltip");
                row.setAttribute("data-bs-placement", "top");
                row.setAttribute("data-bs-title", "Este registro es nuevo");
            }          
        },
        "columns": [
            {
                "data": "Cod_Producto",             
            },
            {
                "data": "Dsc_Producto",
            },
            {
                "data": "Ubicacion_inicial",
                 className: "text-center"
            },
            {
                "data": "Lote_inicial",
                className: "text-center",
                "name": "col_lote_inicial"
            },
            {
                "data": "Ubicacion_contada",
                className: "text-center",
                "title": "Ubicación Contada"
            },
            {
                "data": "Lote_Contado",
                className: "text-center",
                "name": "col_lote_contado"
            },
            {
                "data": "Stock_inicial",
                className: "text-center",
            },
            {
                "data": "Conteo_1",
                className: "text-center",
                "name": "Head_Cont1"
            },
            {
                "data": "Conteo_2",
                className: "text-center",
                "name": "Head_Cont2"
            },
            {
                "data": "Conteo_3",
                className: "text-center",
                "name": "Head_Cont3"
            },
            {
                "data": "Stock_Final",
                className: "text-center",
            },
            {
                "data": "Stock_Diferencial",
                className: "text-center",
            }

        ],
        "footerCallback": function (row, data, start, end, display) {
        },
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        /*"responsive": true,*/
        "autoWidth": false,
        /*deferRender: true,*/
        "language": españolTbl,
        "initComplete": function () {
            aplicarVisibilidadLotes();
        }
    });  
    aplicarEstiloConteoActual();
}

function CargarFooter(TStockInicial, TConteo1, TConteo2, TConteo3, TStockFinal, TStockDiferencial) {
    $('#FCantI').text(TStockInicial);
    $('#FCont1').text(TConteo1);
    $('#FCont2').text(TConteo2);
    $('#FCont3').text(TConteo3);
    $('#FTotal').text(TStockFinal);
    $('#FDif').text(TStockDiferencial);
}


/*// ESTABLECER TIMER RECARGA DE TABLA //*/
const SwitchchkReload = $("#chkReload");
SwitchchkReload.change(function () {
    if (document.getElementById('chkReload').checked) {
        //console.log("Recarga activado");
        cargarInterval()
    } else {
        //console.log("Recarga desactivado");
        clearInterval(nIntervId);
        nIntervId = null;
    }
});
function cargarInterval() {

    let reloadtableGestor = parseInt($('#GestorPrincipal').attr("timer"));

    if (document.getElementById('chkReload').checked) {

        if (!nIntervId) {

            nIntervId = setInterval(function () {

                if (Interval) {

                    Inventario_tabla.ajax.reload(function () {
                        aplicarEstiloConteoActual();
                    }, true);

                }

            }, reloadtableGestor);
        }
    }
}
function RecargarTabla() {
    $('#tbl_Inventario').DataTable().ajax.reload(null, true);
}


/*// OPCIONES AL FINALIZAR CONTEO //*/
$('#ConfirmarConteo').on("click", function () {
    var radSelect = $('input:radio[name=listGroupRadio]:checked').val();

    switch (radSelect) {
        case '1':
            console.log("Opcion 1");                    
            ConteoDiferencial();
            break;
        case '2':
            console.log("Opcion 2");
            ConteoReinicio();
            break;
        case '3':
            console.log("Opcion 3");
            CerrarInventario();
            break;
        default:
    }
    cerrarModalConteo();
    modificarBtnConteo(ConteoActual);
});


/*// CIERRES DE CONTEO //*/
function ConteoDiferencial() {
    let _url = 'ConteoDiferencial'
    var obj = new Object();
    obj.codInventario = Global_CodInventario;
    obj.conteo = ConteoActual;

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        async: false,
        success: function (response) {

            if (response.HUBO_ERROR) {
                Swal.fire("Error  al finalizar el conteo", response.MENSAJE_ERROR, "error");
            }
            else {
                GenerarParticipaciones();
                Swal.fire(`Conteo ${ConteoActual} finalizado`, "Puede continuar con el siguiente conteo con solo las diferencias.", "success");
                    
                cerrarModalConteo();

                OcultarGestion();
                MostrarGestion(Global_CodInventario);
            }
        },
        complete: function (result) {
        },
        error: function (result) {
            Swal.fire({
                icon: "error",
                title: "Error inesperado",
                text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
                footer: `<small>Detalles: ${result}</small>`
            });
        }
    });
}
function ConteoReinicio() {
    let _url = 'ConteoReinicio'
    var obj = new Object();
    obj.codInventario = Global_CodInventario;
    obj.conteo = ConteoActual;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        async: false,
        success: function (response) {
            if (response.HUBO_ERROR) {
                Swal.fire("Error  al finalizar el conteo", response.MENSAJE_ERROR, "error");
            }
            else {
                GenerarParticipaciones();
                Swal.fire(`Conteo ${ConteoActual} finalizado`, "Puede continuar con el siguiente conteo de todos los productos", "success");

                cerrarModalConteo();

                OcultarGestion();
                MostrarGestion(Global_CodInventario);
            }
        }, complete: function (result) {
        },
        error: function (result) {
            Swal.fire({
                icon: "error",
                title: "Error inesperado",
                text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
                footer: `<small>Detalles: ${result}</small>`
            });
        }
    });
}
function CerrarInventario() {
    let _url = 'CerrarIventario'
    var obj = new Object();
    obj.codInventario = Global_CodInventario;
    obj.conteo = ConteoActual;
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        async: false,
        success: function (response) {
            if (response.HUBO_ERROR) {
                console.log("Hubo un error:" + response.HUBO_ERROR);
                Swal.fire("Error  al finalizar el conteo", response.MENSAJE_ERROR, "error");
            }
            else {
                Swal.fire(`Inventario cerrado`, "Se ha finalizado todos los conteos. El inventario ha sido cerrado correctamente", "success");

                cerrarModalConteo();

                OcultarGestion();
                LoadCbxInventario();
            }
        }, complete: function (result) {
        },
        error: function (result) {
            Swal.fire({
                icon: "error",
                title: "Error inesperado",
                text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
                footer: `<small>Detalles: ${result}</small>`
            });
        }
    });
}


/*// ABRIR Y CERRAR EL MODAL DE CONTEO //*/
let modalConteo;
const btnFinalizarConteo = document.getElementById("FinalizarConteo");

btnFinalizarConteo.addEventListener("click", function () {
    
    switch (ConteoActual) {
        case 1:
            abrirModalConteo();            
            break;
        case 2:
            abrirModalConteo();
            break;
        case 3:
            CerrarInventario();           
            break;
    }
});

function modificarBtnConteo(numeroConteo) {
    if (numeroConteo < 3) {
        btnFinalizarConteo.textContent = "Finalizar conteo actual";
        btnFinalizarConteo.classList.add("btn-primary");
        btnFinalizarConteo.classList.remove("btn-danger");
    } else {
        btnFinalizarConteo.textContent = "Cerrar inventario";
        btnFinalizarConteo.classList.remove("btn-primary");
        btnFinalizarConteo.classList.add("btn-danger");
    }
}
function abrirModalConteo() {
    const modalElemento = document.getElementById("ModalConteo");

    if (!modalConteo) {
        modalConteo = new bootstrap.Modal(modalElemento);
    }

    modalConteo.show();
}
function cerrarModalConteo() {
    modalConteo?.hide();
}


/*// HABILITAR Y DESHABILITAR OPCIONES DE CONTEO REPORTE //*/ 
let modalExportConteo;
const btnExportConteo = document.getElementById("btnExportExcel");

if (btnExportConteo) {
    btnExportConteo.addEventListener("click", function () {
        ExportInventario();
        //abrirModalExportConteo();
        //$('#1RadioExcel').prop('checked', true);
        //actualizarRadiosConteo(ConteoActual);
    });
}



//ConteoActual
function actualizarRadiosConteo(conteoActual) {
    for (let i = 1; i <= 3; i++) {
        const $radio = $('#' + i + 'RadioExcel');
        const $label = $('label[for="' + i + 'RadioExcel"]');

        $label.find('.estado-radio').remove();

        if (i <= conteoActual) {
            $radio.prop("disabled", false);
        } else {
            $radio.prop("disabled", true);
            $label.append(
                '<small class="text-muted estado-radio">(no disponible)</small>'
            );
        }
    }
}


/*// GENERAR REPORTE SELECCIONADO //*/
$('#ConfirmarExport').on("click", function () {
    let radSelect = $('input:radio[name=ListRadioExcel]:checked').val();

    switch (radSelect) {
        case "1": ExportInventario(1); break;
        case "2": ExportInventario(2); break;
        case "3": ExportInventario(3); break;
        case "4": ExportInventario(4); break;
        default:
    }
    cerrarModalExportConteo();
});
function abrirModalExportConteo() {
    const modalElemento = document.getElementById("ModalExport");

    if (!modalExportConteo) {
        modalExportConteo = new bootstrap.Modal(modalElemento);
    }

    modalExportConteo.show();
}
function cerrarModalExportConteo() {
    modalExportConteo?.hide();
}


/*// CARGAR RESUMEN CARDS //*/
function CargarCuadrosGraficos(codinventario, Inventariados, Faltantes, Total) {
    $('#dataSquare1').text(codinventario);
    $('#dataSquare2').text(Inventariados);
    $('#dataSquare3').text(Faltantes);
    $('#dataSquare4').text(Total);
}


/*// CARGAR CARDS PRODUCTOS DIFERENCIAL //*/
function CargarGraficoDiferencial(data) {
    // solo me filtra lo que hay en la tabla, ver backend
    let prodConDif = 0;
    let prodSinDif = 0;

    data.forEach((elemento, indice) => {
        if (elemento.Stock_Diferencial !== 0) {
            prodConDif++;
        } else {
            prodSinDif++;
        }
    });

    $('#dataSinDif').text(prodSinDif);
    $('#dataConDif').text(prodConDif);   
}


/*// GRAFICO PIE STOCK TOTAL //*/
let chartPieStock = null;
function GraficoStock(inventariado, faltante) {

    const seriesStock = [parseInt(inventariado), parseInt(faltante)];

    if (chartPieStock) {
        chartPieStock.updateSeries(seriesStock);
        return;
    }

    var options = {
        series: seriesStock,
        chart: {
            fontFamily: "inherit",
            width: 450,
            type: "pie",
        },
        colors: [
            "var(--bs-success)",
            "var(--bs-danger)",
        ],
        labels: ["Stock Inventariado", "Stock Faltante"],
        responsive: [
            {
                breakpoint: 480,
                options: {
                    chart: {
                        width: 300,
                    },
                    legend: {
                        position: "bottom",
                    },
                },
            },
        ],
        legend: {
            labels: {
                colors: "#a1aab2",
            },
        },
    };

    chartPieStock = new ApexCharts(
        document.querySelector("#GraficoPieStock"),
        options
    );
    chartPieStock.render();
}

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
            /*console.log(response);*/
            var json = JSON.parse(response);

            if (json.length > 0) {
                var labels = json.map(function (e) {
                    return e.NOMBRE_USUARIO;
                });
                var data = json.map(function (e) {
                    return e.STOCK_INVENTARIADO;
                });

                DrawnChart1(labels, data, total);
            } else {
                //console.log("No hay datos para el gráfico");
                DrawnChart1([], [], 0);
            }          
        },
        error: function (err) {
            console.error("Error al obtener datos del gráfico", err);
        }
    });
}

let charBarOperadores = null;
function DrawnChart1(labels, data, Total) {

    const seriesOperadores = [{
        name: "Stock enviado",
        data: data
    }];

    //console.log(seriesOperadores);

    if (charBarOperadores) {
        charBarOperadores.updateOptions({
            series: seriesOperadores,
            xaxis: {
                categories: labels
            }
        });
        return;
    }

    var options = {
        series: seriesOperadores,
        chart: {
            fontFamily: "inherit",
            type: "bar",
            width: 450,
            toolbar: {
                show: false,
            },
            foreColor: "#adb0bb",
        },
        grid: {
            borderColor: "transparent",
        },
        colors: [
            "#6f42c1",                                     
            "#20c997",             
            "#fd7e14",   
            "var(--bs-primary)",
            "var(--bs-success)",   
            "var(--bs-warning)",   
            "var(--bs-danger)",    
            "var(--bs-info)"
        ],
        plotOptions: {
            bar: {
                horizontal: true,
                borderRadius: 5,
                columnWidth: "45%",
                distributed: true,
                endingShape: "rounded"
            },
        },
        dataLabels: {
            enabled: true,
        },
        xaxis: {
            categories: labels,
            labels: {
                style: {
                    colors: "#a1aab2",
                },
            },
        },       
        yaxis: {
            labels: {
                style: {
                    colors: "#a1aab2",
                },
            },
        },
        tooltip: {
            theme: "dark",
        },
        noData: {
            text: "No hay datos para mostrar",
            align: "center",
            verticalAlign: "middle",
            style: {
                color: "#a1aab2",
                fontSize: "14px"
            }
        },
        responsive: [
            {
                breakpoint: 480,
                options: {
                    chart: {
                        width: 300,
                    },
                    legend: {
                        position: "bottom",
                    },
                },
            },
        ],
    };

    charBarOperadores = new ApexCharts(
        document.querySelector("#GraficoBarOperadores"),
        options
    );
    charBarOperadores.render();
}

// NUEVO REPARTIR DIFERENCIAS
function GenerarParticipaciones() {
    let _url = 'GenerarParticipaciones';
    var obj = new Object();

    obj.codInventario = Global_CodInventario;
    obj.nroConteoCerrado = ConteoActual;
    obj.minutosLimite = 2;


    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),

        success: function (response) {

            if (response.HUBO_ERROR) {
                console.error(
                    "Error al generar participaciones:",
                    response.MENSAJE_ERROR
                );
            } else {
                console.log(
                    "Participaciones generadas correctamente"
                );
            }
        },

        error: function (result) {
            console.error(
                "Error AJAX al generar participaciones:",
                result
            );
        }
    });

}
