let tblReporteInventario;
let inventarioSeleccionado = {
    codigo: '',
    nombre: ''
};

$(document).ready(function () {
    inicializarComponentes();
    inicializarEventos();
});

function inicializarComponentes() {
    $('#cbxInventariosCerrados').select2({
        theme: 'bootstrap-5',
        width: '100%',
        dropdownParent: $('#modalSelectInventario')
    });
}

function inicializarEventos() {
    $('#btnCargarInventario').click(function () {
        let selectedValue = $('#cbxInventariosCerrados').val();
        let selectedText = $('#cbxInventariosCerrados option:selected').text();

        if (!selectedValue || selectedValue === '-1' || selectedValue === '') {
            Swal.fire({
                icon: 'warning',
                title: 'Atención',
                text: 'Debe seleccionar un inventario válido'
            });
            return;
        }

        inventarioSeleccionado.codigo = selectedValue;
        inventarioSeleccionado.nombre = selectedText;

        $('#modalSelectInventario').modal('hide');

        setTimeout(function () {
            cargarDatosInventario();
        }, 300);
    });

    $('#btnAplicarFiltros').click(function () {
        reloadTableReporte();
    });

    $('#btnLimpiarFiltros').click(function () {
        $('#filtroDiferencias').val('0');
        reloadTableReporte();
    });

    $('#btnExportarExcel').click(function () {
        exportarExcel();
    });
}



function CargarFooterReporte(listFooter) {
    if (listFooter && listFooter.length >= 6) {
        $('#footerStockInicial').text(formatNumber(listFooter[0]));
        $('#footerConteo1').text(formatNumber(listFooter[1]));
        $('#footerConteo2').text(formatNumber(listFooter[2]));
        $('#footerConteo3').text(formatNumber(listFooter[3]));
        $('#footerStockFinal').text(formatNumber(listFooter[4]));

        let diferencial = listFooter[5];
        let diferencialFormatted = formatNumber(diferencial);
        let clase = diferencial > 0 ? 'text-success fw-bold' : diferencial < 0 ? 'text-danger fw-bold' : '';
        $('#footerDiferencial').html('<span class="' + clase + '">' + diferencialFormatted + '</span>');

        // Actualizar resumen con estadísticas adicionales si están disponibles
        if (listFooter.length >= 10) {
            $('#txtResumenConDiferencias').text(formatNumber(listFooter[6]));
            $('#txtResumenSinDiferencias').text(formatNumber(listFooter[7]));
        }
    }
}

function reloadTableReporte() {
    if (typeof tblReporteInventario !== 'undefined') {
        setTimeout(function () {
            tblReporteInventario.ajax.reload(null, false);
        }, 300);
    }
}

function cargarDatosInventario() {
    toggle_Loadingtb('tbLoading', true);

    // Mostrar información básica del resumen
    $('#txtResumenCodInventario').text(inventarioSeleccionado.codigo);
    $('#seccionResumen').removeClass('d-none');

    // Cargar tabla
    CargarTablaReporteInventario(inventarioSeleccionado.codigo);

    // Mostrar sección de reporte
    $('#seccionReporte').removeClass('d-none');

    toggle_Loadingtb('tbLoading', false);
}

function exportarExcel() {
    if (!inventarioSeleccionado.codigo) {
        Swal.fire({
            icon: 'warning',
            title: 'Atención',
            text: 'Debe seleccionar un inventario primero'
        });
        return;
    }

    Swal.fire({
        icon: 'info',
        title: 'Próximamente',
        text: 'La función de exportar a Excel estará disponible pronto'
    });
}

function formatNumber(value) {
    if (value == null || value === '') return '0.00';
    return parseFloat(value).toLocaleString('es-PE', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function toggle_Loadingtb(idLoading, mostrar) {
    if (mostrar) {
        $('#' + idLoading).removeClass('d-none');
    } else {
        $('#' + idLoading).addClass('d-none');
    }
}