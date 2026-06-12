$(document).ready(function () {

    // ===== VALIDACIÓN EN TIEMPO REAL =====
    function validarCampoRequerido(input) {
        const inputElement = $(input);
        const valor = inputElement.val().trim();

        if (valor === '') {
            inputElement.addClass('is-invalid');
            if (inputElement.next('.invalid-feedback').length === 0) {
                inputElement.after('<div class="invalid-feedback">Este campo es requerido</div>');
            }
            return false;
        } else {
            inputElement.removeClass('is-invalid');
            inputElement.next('.invalid-feedback').remove();
            return true;
        }
    }

    $('#txtCod_add, #txtProducto_add').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtCod_edit, #txtProducto_edit').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtCod_add, #txtProducto_add, #txtCod_edit, #txtProducto_edit').on('input', function () {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    });

    // ===== VALIDAR AGREGAR =====
    function ValidarAdd(obj) {
        let valid = true;

        const codValido = validarCampoRequerido('#txtCod_add');
        const descValido = validarCampoRequerido('#txtProducto_add');

        if (!codValido || !descValido) {
            valid = false;
        }

        return valid;
    }

    // ===== LIMPIAR AL CERRAR MODAL AGREGAR =====
    $('#Add_Modal').on('hidden.bs.modal', function () {
        document.getElementById('txtCod_add').value = '';
        document.getElementById('txtProducto_add').value = '';
        $('#cbxUM_add').prop('selectedIndex', 0);
        $('#txtCod_add, #txtProducto_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });

    // ===== BOTÓN AGREGAR =====
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.codProducto = document.getElementById('txtCod_add').value.trim();
        obj.descProducto = document.getElementById('txtProducto_add').value.trim();
        obj.UM = document.getElementById('cbxUM_add').value;

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');

            let _url = "CreateProducto";
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: _url,
                data: JSON.stringify(obj),
                success: function (response) {
                    if (response.HUBO_ERROR) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error al guardar',
                            text: response.MENSAJE_ERROR,
                            confirmButtonColor: '#5d87ff',
                            confirmButtonText: 'Entendido'
                        });
                    } else {
                        // Cerrar modal
                        $('#Add_Modal').modal('hide');

                        // Actualizar tabla
                        ActualizarTabla();

                        // Mostrar éxito
                        Swal.fire({
                            icon: 'success',
                            title: '¡Producto creado exitosamente!',
                            text: 'El producto ha sido registrado correctamente.',
                            confirmButtonColor: '#5d87ff',
                            confirmButtonText: 'Aceptar',
                            //timer: 4500,
                            //timerProgressBar: true
                        });
                    }
                },
                error: function (result) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error de conexión',
                        text: 'No se pudo conectar con el servidor.',
                        confirmButtonColor: '#5d87ff'
                    });
                },
                complete: function () {
                    btnAdd.prop('disabled', false).html('<i class="ti ti-device-floppy me-1"></i>Guardar');
                }
            });
        }
    });

    // ===== BOTÓN TEMPLATE =====
    document.getElementById('btnTemplate').addEventListener('click', function (e) {
        downTemplate();
    });

    // ===== LIMPIAR AL CERRAR MODAL IMPORTAR =====
    $('#Import_Modal').on('hidden.bs.modal', function () {
        $("#fileInput").val("");
    });

});

// ==================== EDICIÓN ====================
var IDTemp = -1;

function editar(id) {
    $('#Edit_Modal').modal('show');
    let _url = "GetProducto";
    var obj = new Object();
    obj.id = id;

    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        success: function (response) {
            var objResponse = response.Entity;
            document.getElementById('txtCod_edit').value = objResponse.vchCodProducto;
            document.getElementById('txtProducto_edit').value = objResponse.vchDescripcion;
            document.getElementById('cbxUM_edit').value = objResponse.intUM;
            document.getElementById('chkActivo_edit').checked = objResponse.intActivo;

            // Limpiar validaciones previas
            $('#txtCod_edit, #txtProducto_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudo cargar la información del producto.',
                confirmButtonColor: '#5d87ff'
            });
        }
    });
}

function validarCampoRequerido(input) {
    const inputElement = $(input);
    const valor = inputElement.val().trim();

    if (valor === '') {
        inputElement.addClass('is-invalid');
        if (inputElement.next('.invalid-feedback').length === 0) {
            inputElement.after('<div class="invalid-feedback">Este campo es requerido</div>');
        }
        return false;
    } else {
        inputElement.removeClass('is-invalid');
        inputElement.next('.invalid-feedback').remove();
        return true;
    }
}

function ValidarEdit(obj) {
    var valid = true;

    const codValido = validarCampoRequerido('#txtCod_edit');
    const descValido = validarCampoRequerido('#txtProducto_edit');

    if (!codValido || !descValido) {
        valid = false;
    }

    return valid;
}

$('#Edit_Modal').on('hidden.bs.modal', function () {
    $('#txtCod_edit, #txtProducto_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.id = IDTemp;
    obj.codProducto = document.getElementById('txtCod_edit').value.trim();
    obj.descProducto = document.getElementById('txtProducto_edit').value.trim();
    obj.UM = document.getElementById('cbxUM_edit').value;
    obj.activo = document.getElementById('chkActivo_edit').checked;

    let _url = "UpdateProducto";
    var _valid = ValidarEdit(obj);

    if (_valid) {
        const btnSave = $('#btn_SaveEdit');
        btnSave.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');

        $.ajax({
            type: "Post",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: _url,
            data: JSON.stringify(obj),
            success: function (response) {
                if (response.HUBO_ERROR) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error al actualizar',
                        text: response.MENSAJE_ERROR,
                        confirmButtonColor: '#5d87ff',
                        confirmButtonText: 'Entendido'
                    });
                } else {
                    $('#Edit_Modal').modal('hide');
                    ActualizarTabla();

                    Swal.fire({
                        icon: 'success',
                        title: '¡Producto actualizado!',
                        text: 'El producto se ha actualizado correctamente.',
                        confirmButtonColor: '#5d87ff',
                        confirmButtonText: 'Aceptar',
                        //timer: 4500,
                        //timerProgressBar: true
                    });
                }
            },
            error: function (result) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error de conexión',
                    text: 'No se pudo conectar con el servidor.',
                    confirmButtonColor: '#5d87ff'
                });
            },
            complete: function () {
                btnSave.prop('disabled', false).html('<i class="ti ti-device-floppy me-1"></i>Guardar');
            }
        });
    }
}

// ==================== IMPORTACIÓN API ====================
$("#Prod_Almacen").on("click", function () {
    $("#Prod_Almacen").attr("disabled", true);

    // Mostrar SweetAlert de carga
    Swal.fire({
        title: 'Importando productos...',
        html: 'Por favor espere mientras se cargan los datos.',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    ApiCarga();
});

function ApiCarga() {
    let _url = "CargarProductosAPIExterna";
    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(),
        success: function (response) {
            if (response.HUBO_ERROR) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error en la carga',
                    text: response.MENSAJE_ERROR,
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Entendido'
                });
            } else {
                Swal.fire({
                    icon: 'success',
                    title: '¡Carga exitosa!',
                    text: response.MENSAJE_ERROR || 'Los productos se han cargado correctamente.',
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Aceptar',
                    //timer: 4500,
                    //timerProgressBar: true
                });
                ActualizarTabla();
            }
        },
        complete: function () {
            $("#Prod_Almacen").attr("disabled", false);
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error de conexión',
                text: 'No se pudo conectar con el servidor.',
                confirmButtonColor: '#5d87ff'
            });
        }
    });
}