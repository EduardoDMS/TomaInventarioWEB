$(document).ready(function () {

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

    $('#txtCodALmacen_add, #txtDscALmacen_add').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtdscAlmacen_edit').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtCodALmacen_add, #txtDscALmacen_add, #txtdscAlmacen_edit').on('input', function () {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    });

    function ValidarAdd(obj) {
        let valid = true;
        const codAlmacenValido = validarCampoRequerido('#txtCodALmacen_add');
        const descAlmacenValido = validarCampoRequerido('#txtDscALmacen_add');

        if (!codAlmacenValido || !descAlmacenValido) {
            valid = false;
        }
        return valid;
    }

    // Limpiar al cerrar modal
    $('#Add_almacen').on('hidden.bs.modal', function () {
        document.getElementById('txtCodALmacen_add').value = '';
        document.getElementById('txtDscALmacen_add').value = '';
        $('#txtCodALmacen_add, #txtDscALmacen_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });

    // Botón Agregar
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.codAlmacen = document.getElementById('txtCodALmacen_add').value.trim();
        obj.descAlmacen = document.getElementById('txtDscALmacen_add').value.trim();

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');

            let _url = "CreateAlmacen";
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
                        $('#Add_almacen').modal('hide');
                        ActualizarTabla();

                        Swal.fire({
                            icon: 'success',
                            title: '¡Almacén creado exitosamente!',
                            text: 'El almacén ha sido registrado correctamente.',
                            confirmButtonColor: '#5d87ff',
                            confirmButtonText: 'Aceptar',
                            //timer: 5000,
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
});

// ==================== EDICIÓN ====================
var IDTemp = -1;

function editar(id) {
    $('#Edit_almacen').modal('show');
    let _url = "GetAlmacen";
    var obj = new Object();
    obj.idAlmacen = id;

    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        success: function (response) {
            var objResponse = response.Entity;
            document.getElementById('txtcodAlmacen_edit').value = objResponse.vchcodAlmacen;
            document.getElementById('txtdscAlmacen_edit').value = objResponse.vchdscAlmacen;
            document.getElementById('chkActivo_edit').checked = objResponse.intActivo;

            $('#txtdscAlmacen_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudo cargar la información del almacén.',
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
    const descAlmacenValido = validarCampoRequerido('#txtdscAlmacen_edit');

    if (!descAlmacenValido) {
        valid = false;
    }
    return valid;
}

$('#Edit_almacen').on('hidden.bs.modal', function () {
    $('#txtdscAlmacen_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.idAlmacen = IDTemp;
    obj.codAlmacen = document.getElementById('txtcodAlmacen_edit').value;
    obj.dscAlmacen = document.getElementById('txtdscAlmacen_edit').value.trim();
    obj.activo = document.getElementById('chkActivo_edit').checked;

    let _url = "UpdateAlmacen";
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
                    $('#Edit_almacen').modal('hide');
                    ActualizarTabla();

                    Swal.fire({
                        icon: 'success',
                        title: '¡Almacén actualizado exitosamente!',
                        text: 'Los cambios han sido guardados correctamente.',
                        confirmButtonColor: '#5d87ff',
                        confirmButtonText: 'Aceptar',
                        //timer: 5000,
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
$("#Api_Almacen").on("click", function () {
    $("#Api_Almacen").attr("disabled", true);

    Swal.fire({
        title: 'Importando almacenes...',
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
    let _url = "CargarAlmacenesAPIExterna";
    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(),
        success: function (response) {
            console.log(response);

            if (response.HUBO_ERROR) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error en la importación',
                    text: response.MENSAJE_ERROR,
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Entendido'
                });
            } else {
                Swal.fire({
                    icon: 'success',
                    title: '¡Importación exitosa!',
                    text: response.MENSAJE_ERROR || 'Los almacenes se han importado correctamente.',
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Aceptar',
                    //timer: 5000,
                    //timerProgressBar: true
                });
                ActualizarTabla();
            }
        },
        complete: function () {
            $("#Api_Almacen").attr("disabled", false);
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