var tabla;
tabla = $('#dtTabla').DataTable({
    "ajax": {
        "url": "ListarTipoMonedas",
        "type": "POST",
        "datatype": "json",
        "data": function (f) {
            var txtMoneda = document.getElementById("txtMoneda").value;
            var cbxEstado = document.getElementById("cbxActivo").value;
            f.codMoneda = txtMoneda;
            f.dscMoneda = "";
            f.activo = cbxEstado;
        },
        "complete": function (response) {
            //if (Swal.isVisible()) {
            //    Swal.close();
            //}
            if (Swal.isVisible() && !Swal.getIcon()) {
                Swal.close();
            }

            if (response.responseJSON.HUBO_ERROR) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Ocurrió un error al cargar los datos',
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000
                });
            }
        }
    },
    "columns": [
        {
            "data": null,
            "orderable": false,
            "searchable": false,
            "render": function (data, type, row, meta) {
                return '<button class="btn btn-link btn-editar text-primary">' +
                    '<i class="ti ti-edit fs-5"></i>' +
                    '</button>';
            },
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).find('.btn-editar').click(function (event) {
                    event.stopPropagation();
                    editar(rowData.idMoneda);
                });
            }
        },
        //{
        //    "data": "idMoneda"
        //},
        {
            "data": "codMoneda"
        },
        {
            "data": "dscMoneda"
        },
        {
            "data": "flgActivo",
            "render": function (data, type, row, meta) {
                if (data === true || data === "Activo" || data === "1" || data === 1) {
                    return '<span class="badge bg-success-subtle text-success">' +
                        '<i class="ti ti-circle-filled fs-1"></i> Activo</span>';
                }

                return '<span class="badge bg-danger-subtle text-danger">' +
                    '<i class="ti ti-circle-filled fs-1"></i> Inactivo</span>';
            }
        }
    ],
    "columndefs": [
        {
            "targets": -1,
            "classname": "col_btns"
        }
    ],
    "paging": true,
    "pageLength": 10,
    "searching": false,
    "lengthChange": true
    //,
    //"language": españolTbl
});

$('#btn_filtrar').on('click', function () {
    Swal.fire({
        title: 'Buscando...',
        text: 'Por favor espere',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    ActualizarTabla();
});

$('#txtMoneda').on('keypress', function (e) {
    if (e.which === 13) {
        e.preventDefault();
        $('#btn_filtrar').click();
    }
});

$('#btn_limpiar').on('click', function () {
    $('#txtMoneda').val('');
    $('#cbxActivo').val('');

    Swal.fire({
        title: 'Limpiando filtros...',
        text: 'Por favor espere',
        allowOutsideClick: false,
        allowEscapeKey: false,
        showConfirmButton: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    ActualizarTabla();
});

function ActualizarTabla() {
    //tabla.ajax.reload(function () {
    //    Swal.close();
    //},true);

    setTimeout(function () {
        tabla.ajax.reload(null, true);
        Swal.close();
    }, 500);
}
