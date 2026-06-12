var tb_usuario;


    tb_usuario = $('#dtUsuario').DataTable({
        //"processing": true,
        //"serverSide": true,
        //"responsive": true,

        "ajax": {
            "url": "ListarUsuarios",
            "type": "POST",
            "datatype": "json",
            "data": function (f) {

                var txtUsuario = document.getElementById("txtUsuario").value;
                var cbxPerfil = document.getElementById("cbxPerfil").value;

                f.perfil = cbxPerfil
                f.usuario = txtUsuario
            },
                //toggle_Loadingtb('tbLoading', true);
                ////Console.log(f);
                "complete": function (response) {
                    // Solo cierra el Swal si es un loading, no uno de éxito/error
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
            //}, "complete": function (response) {
            //    if (Swal.isVisible()) {
            //        Swal.close();
            //    }

            //    if (response.responseJSON.HUBO_ERROR) {
            //        Swal.fire({
            //            icon: 'error',
            //            title: 'Error',
            //            text: 'Ocurrió un error al cargar los datos',
            //            toast: true,
            //            position: 'top-end',
            //            showConfirmButton: false,
            //            timer: 3000
            //        });
            //    }
            //}
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
                    var id = row.IdUsuario;
                    //var nombre = row.vch_nombreusuario;
                    //var txtbotoneliminar = textoboton(row.estado);
                    //var textobotoneliminar = textoestado(estado);
                    //return '<button   class="btn   btn-link "  onclick="editar(' + id + ');" ><i class="fa-solid fa-user-pen fa-xl"></i></button>'
                    return '<button   class="btn btn-link btn-editar text-primary"  ><i class="ti ti-edit fs-5"></i></button>'

                },
                "createdCell": function (td, cellData, rowData, row, col) {
                    $(td).find('.btn-editar').click(function (event) {
                        event.stopPropagation();
                        editar(rowData.IdUsuario);
                    });
                }
            },
            { "data": "Usuario" },
            { "data": "Perfil" },
            /*{ "data": "vchActivo" }*/
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

    $('#txtUsuario').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            $('#btn_filtrar').click();
        }
    });


$('#btn_limpiar').on('click', function () {
    $('#txtUsuario').val('');
    $('#cbxPerfil').prop('selectedIndex', 0); // Para seleccionar la primera opción

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
    ////Console.log("Se Actualiza")
    setTimeout(function () {
        tb_usuario.ajax.reload(null, true);
        Swal.close();
    }, 500);
}