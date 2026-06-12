//$(document).ready(function () {
//    toggle_Loadingtb('tbLoading', false);


$(document).ready(function () {
    //toggle_Loadingtb('tbLoading', false);
    // POR:
    //Swal.fire({
    //    title: 'Cargando...',
    //    text: 'Por favor espere',
    //    allowOutsideClick: false,
    //    didOpen: () => {
    //        Swal.showLoading();
    //    }
    //});


    

//});
    

});
var init = true
function destroyTable() {
    if (!init) {
        ////Console.log('Destruir tablas');
        $('#tbl_almacen').DataTable().destroy();
        $('#tbl_usuarios').DataTable().destroy();
        $('#tbl_usuXalmn').DataTable().destroy();
        $('#tbl_productos').DataTable().destroy();
        $('#tbl_ubicacion').DataTable().destroy();

        //Tablas de Inventario
        $('#tbl_inventario').DataTable().destroy();
        $('#tbl_detInventario').DataTable().destroy();
    }
    init = false;
}

function CargarTablasMaestro(response) {
    $('#cardtables').css('visibility', 'visible');
    destroyTable();
    tblAlmacen(response[0].Entity);
    tblUsuarios(response[1].Entity);
    tblUsuXAlM(response[2].Entity);
    tblProductos(response[3].Entity);
    tblUbicacion(response[4].Entity);
}
function CargarTablasInventario(response) {
    $('#cardtables').css('visibility', 'visible');
    tblInventario(response[0].Entity);
    /*if (response[1].HUBO_ERROR == false) {*/
        tblDetInventario(response[1].Entity);
    //}
    
}

function tblAlmacen(Jsondata) {
    ////Console.log(Jsondata);
    //var tb = $('#tbl_almacen').DataTable();
    //tb.destroy();
    RImport_tabla = $('#tbl_almacen').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {
            
        },
        "columns": [
            { "data": "Objeto.vchcodAlmacen", "title": "Cod_Almacen" },
            { "data": "Objeto.vchdscAlmacen", "title": "Desc_Almacen" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}

function tblUsuarios(Jsondata) {
    ////Console.log(Jsondata);
    RImport_tabla = $('#tbl_usuarios').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {

        },
        "columns": [
            { "data": "Objeto.Usuario", "title": "Cod_Usuario" },
            { "data": "Objeto.Perfil", "title": "Perfil" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}

function tblUsuXAlM(Jsondata) {
    ////Console.log(Jsondata);
    
    RImport_tabla = $('#tbl_usuXalmn').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {

        },
        "columns": [
            { "data": "Objeto.Usuario", "title": "Cod_Usuario" },
            { "data": "Objeto.Perfil", "title": "Cod_Almacen" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}

function tblProductos(Jsondata) {
    ////Console.log(Jsondata);
   
    RImport_tabla = $('#tbl_productos').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {

        },
        "columns": [
            { "data": "Objeto.vchCodProducto", "title": "Cod_Producto" },
            { "data": "Objeto.vchDescripcion", "title": "Desc_Producto" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}

function tblUbicacion(Jsondata) {
    ////Console.log(Jsondata);
    RImport_tabla = $('#tbl_ubicacion').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {

        },
        "columns": [
            { "data": "Objeto.vchCOD_Almacen", "title": "Cod_Almacen" },
            { "data": "Objeto.vchCod_Ubicacion", "title": "Cod_Ubicación" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}

function tblInventario(Jsondata) {
    ////Console.log(Jsondata);
    RImport_tabla = $('#tbl_inventario').DataTable({
        "data": Jsondata,
        "createdRow": function (row, data, dataIndex) {

        },
        "columns": [
            { "data": "Objeto.Cod_Inventario", "title": "Cod_Inventario" },
            { "data": "Objeto.Cod_Almacen", "title": "Cod_Almacen" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }


                //    return stringResult

                //}
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

        "language": españolTbl
    });
}
function tblDetInventario(Jsondata) {
    ////Console.log(Jsondata);
    RImport_tabla = $('#tbl_detInventario').DataTable({
        "data": Jsondata,
        "serverSide": true,
        "ajax": {
            "url": "Select_DET_INV_IMPORT",
            "type": "POST",
            "datatype": "json",
            "data": function (f) {

                f.searchValue = $('input[type="search"][aria-controls="tbl_detInventario"]').val();

                //Console.log(f);
                Swal.fire({
                    title: 'Cargando...',
                    text: 'Por favor espere',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
            },
            "complete": function (response) {
                Swal.close();
            }
        },
        "createdRow": function (row, data, dataIndex) {
        },
        "columns": [
            { "data": "Objeto.Cod_Inventario", "title": "Cod_Inventario" },
            { "data": "Objeto.Cod_Ubicacion", "title": "Cod_Ubicación" },
            { "data": "Objeto.Cod_Producto", "title": "Cod_Producto" },
            { "data": "Objeto.Lote_Prod", "title": "Lote_Producto" },
            //{ "data": "Objeto.Serie_Prod", "title": "Serie_Producto" },
            { "data": "Objeto.Stock_Inicial", "title": "Stock_Inicial" },
            { "data": "Mensaje", "title": "Mensaje" },
            {
                "data": null,
                "orderable": false,
                "searchable": false,
                "title": "Resultado",
                //"render": function (data, type, row, meta) {
                //    var stringResult = '';
                //    if (data.Flg_pass == 0) {
                //        stringResult = '<i class="fa-solid fa-xmark fa-xl" style="color: #ff0000;"></i>';
                //    } else { stringResult = '<i class="fa-solid fa-check fa-xl" style="color: #56eb11;"></i>'; }
                //    return stringResult
                //}
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
        "language": españolTbl
    });
}
//function tblDetInventario(Jsondata) {
//    ////Console.log(Jsondata);
//    RImport_tabla = $('#tbl_detInventario').DataTable({
//        "data": Jsondata,
//        "serverSide": true,
//        "ajax": {
//            "url": "Select_DET_INV_IMPORT",
//            "type": "POST",
//            "datatype": "json",
//            "data": function (f) {
                
//                f.searchValue = $('input[type="search"][aria-controls="tbl_detInventario"]').val();
                
//                //Console.log(f);
//                toggle_Loadingtb('tbLoading', true);
//            }, "complete": function (response) {
//                //    toggle_Loadingtb('tbLoading', false);
//                Swal.close();
//            }
//        },
//        "createdRow": function (row, data, dataIndex) {

//        },
//        "columns": [
//            { "data": "Objeto.Cod_Inventario", "title": "Cod_Inventario" },
//            { "data": "Objeto.Cod_Ubicacion", "title": "Cod_Ubicación" },
//            { "data": "Objeto.Cod_Producto", "title": "Cod_Producto" },
//            { "data": "Objeto.Lote_Prod", "title": "Lote_Producto" },
//            //{ "data": "Objeto.Serie_Prod", "title": "Serie_Producto" },
//            { "data": "Objeto.Stock_Inicial", "title": "Stock_Inicial" },
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
//}
