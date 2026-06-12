var tabla;

    if (typeof tabla !== 'undefined') {
        ////Console.log('destruir');
        tabla.destroy();
    }

    tabla = $('#dtTabla').DataTable({
        //"processing": true,
        "serverSide": true,
        //"responsive": true,

        "ajax": {
            "url": "ListarProductos",
            "type": "POST",
            "datatype": "json",
            "data": function (f) {

                var txtAlmacen = document.getElementById("txtProducto").value;
                var cbxEstado = document.getElementById("cbxActivo").value;

                f.vchProducto = txtAlmacen
                f.activo = cbxEstado
                ////Console.log(f);
            }, "complete": function (response) {
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
        }, "createdRow": function (row, data, dataIndex) {

            //if (data.estado === "Inactivo") {

            //    $(row).addClass('rowInactive');
            //    $(row).find('.btnDelete').prop('disabled', true);

            //}
        },
        "columns": [
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "render": function (data, type, row, meta) {

                    //var estado = row.status;
                    var id = row.idProducto;
                    //var nombre = row.vch_nombreusuario;
                    //var txtbotoneliminar = textoboton(row.estado);
                    //var textobotoneliminar = textoestado(estado);
                    //return '<button   class="btn   btn-link "  onclick="editar(' + id + ');" ><i class="fa-solid fa-pen-to-square fa-xl"></i></button>'
                    return '<button class="btn btn-link btn-editar text-primary"><i class="ti ti-edit fs-5"></i></button>'

                },
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).find('.btn-editar').click(function (event) {
                        event.stopPropagation();
                        editar(rowData.idProducto);
                    });
                }
            },
            { "data": "vchCodProducto" },
            { "data": "vchDescripcion" },
            {
                "data": "vchActivo",
                "render": function (data, type, row, meta) {
                    if (data === "Activo" || data === "1" || data === 1) {
                        return '<span class="badge bg-success-subtle text-success"><i class="ti ti-circle-filled fs-1"></i> Activo</span>';
                    } else {
                        return '<span class="badge bg-danger-subtle text-danger"><i class="ti ti-circle-filled fs-1"></i> Inactivo</span>';
                    }
                }
            }

        ], "columndefs": [
            {
                "targets": -1, // última columna
                "classname": "col_btns"
            }
        ],
        "paging": true,
        "pageLength": 10,
        "searching": false,
        "lengthChange": true,
        "responsive": true,

        "language": españolTbl
    });

    //$('#btn_filtrar').on('click', function () {

    //    ActualizarTabla();
//});

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


$('#txtProducto').on('keypress', function (e) {
    if (e.which === 13) {
        e.preventDefault();
        $('#btn_filtrar').click();
    }
});

$('#btn_limpiar').on('click', function () {
    $('#txtProducto').val('');
    $('#cbxActivo').prop('selectedIndex', 0); // Para seleccionar la primera opción


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

    function textoBoton(estado) {
        var icono = '<i class="fa-regular fa-circle-xmark fa-xl" ></i>';
        if (estado == 'Activo') { icono = '<i class="fa-regular fa-circle-check  fa-xl" ></i>' }

        return icono;
    }


function ActualizarTabla() {
    ////Console.log("Se Actualiza")
    setTimeout(function () {
        tabla.ajax.reload(null, true);
        Swal.close();
    }, 500);
}

function Charge_RImport_Modal(Jsondata) {
    // Cerrar el modal de importación primero
    const importModal = document.getElementById('Import_Modal');
    const bsImportModal = bootstrap.Modal.getInstance(importModal);

    if (bsImportModal) {
        bsImportModal.hide();
    } else {
        $('#Import_Modal').modal('hide');
    }

    // Esperar a que el modal de importación se cierre completamente
    $(importModal).one('hidden.bs.modal', function () {
        // Limpiar backdrop si quedó
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
        $('body').css('overflow', '');
        $('body').css('padding-right', '');

        // Destruir instancia previa de DataTable si existe
        if ($.fn.DataTable.isDataTable('#RImport_Tbl')) {
            $('#RImport_Tbl').DataTable().destroy();
        }

        // Crear la nueva DataTable con los resultados
        RImport_tabla = $('#RImport_Tbl').DataTable({
            "data": Jsondata,
            "createdRow": function (row, data, dataIndex) {
                if (data.Flg_pass == 0) {
                    $(row).addClass('table-danger');
                } else {
                    $(row).addClass('table-success');
                }
            },
            "columns": [
                {
                    "data": "Objeto.vchCodProducto",
                    "title": "Código",
                    "className": "fw-semibold"
                },
                {
                    "data": "Objeto.vchDescripcion",
                    "title": "Descripción"
                },
                {
                    "data": "Objeto.intUM",
                    "title": "U.M.",
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        return data || '-';
                    }
                },
                {
                    "data": "Mensaje",
                    "title": "Mensaje",
                    "className": "text-start"
                },
                {
                    "data": null,
                    "orderable": false,
                    "searchable": false,
                    "title": "Estado",
                    "className": "text-center",
                    "render": function (data, type, row, meta) {
                        if (data.Flg_pass == 0) {
                            return '<span class="badge bg-danger"><i class="ti ti-x fs-4"></i> Error</span>';
                        } else {
                            return '<span class="badge bg-success"><i class="ti ti-check fs-4"></i> Éxito</span>';
                        }
                    }
                }
            ],
            "paging": true,
            "pageLength": 10,
            "searching": true,
            "lengthChange": true,
            "responsive": true,
            "language": españolTbl,
            "order": [[4, "asc"]] // Ordenar por estado (errores primero)
        });

        // Mostrar el modal de resultados después de cerrar el de importación
        $('#RImport_modal').modal('show');
    });
}
