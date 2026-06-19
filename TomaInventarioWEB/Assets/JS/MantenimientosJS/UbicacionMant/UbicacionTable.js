//var table;

//if (typeof tabla !== 'undefined') {
//    ////Console.log('destruir');
//    tabla.destroy();
//}


//    table = $('#dtTabla').DataTable({
//        //"processing": true,
//        "serverSide": true,
//        //"responsive": true,

//        "ajax": {
//            "url": "ListarUbicaciones",
//            "type": "POST",
//            "datatype": "json",
//            "data": function (f) {

//                var cbxactivo = document.getElementById("cbxActivo").value;
//                var txtvchUbicacion = document.getElementById("txtUbicacion").value;
//                var cbxidAlmacen = document.getElementById("cbxAlmacen").value;

//                f.activo = cbxactivo
//                f.vchUbicacion = txtvchUbicacion
//                f.idAlmacen = cbxidAlmacen
//                toggle_Loadingtb('tbLoading', true);
//                ////Console.log(f);
//            }, "complete": function (response) {
//                toggle_Loadingtb('tbLoading', false);
//                if (response.responseJSON.HUBO_ERROR) {
//                    //Console.log("Error")
//                }

//            }
//        }, "createdRow": function (row, data, dataIndex) {

//            //if (data.estado === "Inactivo") {

//            //    $(row).addClass('rowInactive');
//            //    $(row).find('.btnDelete').prop('disabled', true);

//            //}
//        },
//        "columns": [
//            {
//                "data": null,
//                "orderable": false,
//                "searchable": false,
//                "render": function (data, type, row, meta) {

//                    //var estado = row.status;
//                    var id = row.vchCod_Ubicacion;
//                    //var nombre = row.vch_nombreusuario;
//                    //var txtbotoneliminar = textoboton(row.estado);
//                    //var textobotoneliminar = textoestado(estado);
//                    /*return '<button   class="btn   btn-link "  onclick="editar(\''+id+'\');" ><i class="fa-solid fa-pen-to-square fa-xl"></i></button>'*/
//                    return '<button   class="btn btn-link btn-editar"  ><i class="fa-solid fa-pen-to-square fa-xl"></i></button>'
//                },
//                "createdCell": function (td, cellData, rowData, row, col) {
//                    $(td).find('.btn-editar').click(function (event) {
//                        event.stopPropagation();
//                        editar(rowData.vchCod_Ubicacion);
//                    });
//                }
//            },

//            { "data": "vchCod_Ubicacion" },
//            { "data": "vchDSC_Ubicacion" },
//            { "data": "vchActivo" },
//            { "data": "vchDSC_Almacen" }

//        ], "columndefs": [
//            {
//                "targets": -1, // última columna
//                "classname": "col_btns"
//            }
//        ],
//        "paging": true,
//        "pageLength": 10,
//        "searching": false,
//        "lengthChange": true,
//        "responsive": true,

//        "language": españolTbl
//    });

//    $('#btn_filtrar').on('click', function () {

//        ActualizarTabla();
//    });

//    function textoBoton(estado) {
//        var icono = '<i class="fa-regular fa-circle-xmark fa-xl" ></i>';
//        if (estado == 'Activo') { icono = '<i class="fa-regular fa-circle-check  fa-xl" ></i>' }

//        return icono;
//    }


//function ActualizarTabla() {
//    ////Console.log("Se Actualiza")
//    setTimeout(function () {
//        table.ajax.reload(null, true);
//    }, 500);
//}

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
//            { "data": "Objeto.vchCOD_Almacen", "title": "Cod_Almacen" },
//            { "data": "Objeto.vchCod_Ubicacion", "title": "Cod_Ubicación" },
//            { "data": "Objeto.vchDSC_Ubicacion", "title": "Descripción" },
//            { "data": "Mensaje", "title": "Mensaje" },
//            {
//                "data": null,
//                "orderable": false,
//                "searchable": false,
//                "title": "Resultado",
//                "render": function (data, type, row, meta) {
//                    var stringResult = '';
//                    if (data.Flg_pass == 0) {
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
//    }


var table;

if (typeof tabla !== 'undefined') {
    ////Console.log('destruir');
    tabla.destroy();
}


table = $('#dtTabla').DataTable({
    //"processing": true,
    "serverSide": true,
    //"responsive": true,

    "ajax": {
        "url": "ListarUbicaciones",
        "type": "POST",
        "datatype": "json",
        "data": function (f) {

            var cbxactivo = document.getElementById("cbxActivo").value;
            var txtvchUbicacion = document.getElementById("txtUbicacion").value;
            var cbxidAlmacen = document.getElementById("cbxAlmacen").value;

            f.activo = cbxactivo
            f.vchUbicacion = txtvchUbicacion
            f.idAlmacen = cbxidAlmacen
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
                var id = row.vchCod_Ubicacion;
                //var nombre = row.vch_nombreusuario;
                //var txtbotoneliminar = textoboton(row.estado);
                //var textobotoneliminar = textoestado(estado);
                /*return '<button   class="btn   btn-link "  onclick="editar(\''+id+'\');" ><i class="fa-solid fa-pen-to-square fa-xl"></i></button>'*/
                return '<button class="btn btn-link btn-editar text-primary"><i class="ti ti-edit fs-5"></i></button>'
            },
            "createdCell": function (td, cellData, rowData, row, col) {
                $(td).find('.btn-editar').click(function (event) {
                    event.stopPropagation();
                    editar(rowData.vchCod_Ubicacion);
                });
            }
        },

        { "data": "vchCod_Ubicacion" },
        { "data": "vchDSC_Ubicacion" },
        {
            "data": "vchActivo",
            "render": function (data, type, row, meta) {
                if (data === "Activo" || data === "1" || data === 1) {
                    return '<span class="badge bg-success-subtle text-success"><i class="ti ti-circle-filled fs-1"></i> Activo</span>';
                } else {
                    return '<span class="badge bg-danger-subtle text-danger"><i class="ti ti-circle-filled fs-1"></i> Inactivo</span>';
                }
            }
        },
        { "data": "vchDSC_Almacen" }

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


$('#txtUbicacion').on('keypress', function (e) {
    if (e.which === 13) {
        e.preventDefault();
        $('#btn_filtrar').click();
    }
});

function textoBoton(estado) {
    var icono = '<i class="fa-regular fa-circle-xmark fa-xl" ></i>';
    if (estado == 'Activo') { icono = '<i class="fa-regular fa-circle-check  fa-xl" ></i>' }

    return icono;
}

$('#btn_limpiar').on('click', function () {
    $('#txtUbicacion').val('');
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
    ////Console.log("Se Actualiza")
    setTimeout(function () {
        table.ajax.reload(null, true);
        Swal.close();
    }, 500);
}


function Charge_RImport_Modal(Jsondata) {
    $('#RImport_modal').modal('show');

    // Destruir instancia previa si existe
    if ($.fn.DataTable.isDataTable('#RImport_Tbl')) {
        $('#RImport_Tbl').DataTable().destroy();
    }

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
                "data": "Objeto.vchCOD_Almacen",
                "title": "Cód. Almacén"
            },
            {
                "data": "Objeto.vchCod_Ubicacion",
                "title": "Cód. Ubicación"
            },
            {
                "data": "Objeto.vchDSC_Ubicacion",
                "title": "Descripción"
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
                //"render": function (data, type, row, meta) {
                //    if (data.Flg_pass == 0) {
                //        return '<i class="fas fa-times-circle fa-lg text-danger" title="Error"></i>';
                //    } else {
                //        return '<i class="fas fa-check-circle fa-lg text-success" title="Éxito"></i>';
                //    }
                //}
            }
        ],
        "paging": true,
        "pageLength": 10,
        "searching": true,
        "lengthChange": true,
        "responsive": true,
        "language": españolTbl,
        "order": [[4, "desc"]] // Ordenar por estado (errores primero)
    });
}
