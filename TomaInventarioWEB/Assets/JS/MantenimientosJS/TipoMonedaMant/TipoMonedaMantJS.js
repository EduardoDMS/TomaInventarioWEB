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

    $('#txtCodMoneda_add, #txtDscMoneda_add').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtdscMoneda_edit').on('blur', function () {
        validarCampoRequerido(this);
    });

    $('#txtCodMoneda_add, #txtDscMoneda_add, #txtdscMoneda_edit').on('input', function () {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    });

    function ValidarAdd(obj) {
        let valid = true;
        const codMonedaValido = validarCampoRequerido('#txtCodMoneda_add');
        const descAlmacenValido = validarCampoRequerido('#txtDscMoneda_add');

        if (!codMonedaValido || !descAlmacenValido) {
            valid = false;
        }
        return valid;
    }

    // Limpiar al cerrar modal
    $('#Add_TipoMoneda').on('hidden.bs.modal', function () {
        document.getElementById('txtCodMoneda_add').value = '';
        document.getElementById('txtDscMoneda_add').value = '';
        $('#txtCodMoneda_add, #txtDscMoneda_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });

    // Botón Agregar
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.codMoneda = document.getElementById('txtCodMoneda_add').value.trim();
        obj.dscMoneda = document.getElementById('txtDscMoneda_add').value.trim();

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');

            let _url = "InsertarTipoMoneda";
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
                        $('#Add_TipoMoneda').modal('hide');

                        // Mostrar éxito
                        setTimeout(() => {
                            Swal.fire({
                                icon: 'success',
                                title: '¡Nuevo valor de moneda creado exitosamente!',
                                text: 'El nuevo cambio ha sido registrado correctamente.',
                                confirmButtonColor: '#5d87ff',
                                confirmButtonText: 'Aceptar',
                                timer: 5000,
                                timerProgressBar: true,
                                showConfirmButton: true,
                                allowOutsideClick: true
                            }).then((result) => {
                                if (result.isConfirmed || result.isDismissed) {
                                    // Actualizar tabla
                                    ActualizarTabla();
                                }
                            });
                        }, 100);
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
    $('#Edit_Moneda').modal('show');
    let _url = "ObtenerTipoMonedaId";
    var obj = new Object();
    obj.idTipoMoneda = id;

    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        success: function (response) {
            var objResponse = response.Entity;
            document.getElementById('txtcodMoneda_edit').value = objResponse.codMoneda;
            document.getElementById('txtdscMoneda_edit').value = objResponse.dscMoneda; /////////////////////
            document.getElementById('chkActivo_edit').checked = objResponse.flgActivo;

            $('#txtdscMoneda_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudo cargar la información del TipoeMoneda.',
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
    const descAlmacenValido = validarCampoRequerido('#txtdscMoneda_edit');

    if (!descAlmacenValido) {
        valid = false;
    }
    return valid;
}

$('#Edit_Moneda').on('hidden.bs.modal', function () {
    $('#txtdscMoneda_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.idMoneda = IDTemp;
    obj.codMoneda = document.getElementById('txtcodMoneda_edit').value; // ERROR_COD
    obj.dscMoneda = document.getElementById('txtdscMoneda_edit').value.trim();
    obj.flgActivo = document.getElementById('chkActivo_edit').checked;

    let _url = "EditarTipoMoneda";
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
                    $('#Edit_Moneda').modal('hide');

                    // Mostrar éxito
                    setTimeout(() => {
                        Swal.fire({
                            icon: 'success',
                            title: '¡Tipo de Moneda actualizado exitosamente!',
                            text: 'Los cambios han sido guardados correctamente.',
                            confirmButtonColor: '#5d87ff',
                            confirmButtonText: 'Aceptar',
                            timer: 5000,
                            timerProgressBar: true,
                            showConfirmButton: true,
                            allowOutsideClick: true
                        }).then((result) => {
                            if (result.isConfirmed || result.isDismissed) {
                                // Actualizar tabla
                                ActualizarTabla();
                            }
                        });
                    }, 100);
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
//
//
//  ====================================================================Averiguar funcionalidad amor ============================================
//  
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