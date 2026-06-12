$(document).ready(function () {

    // ===== SOLO PERMITIR NÚMEROS EN txtCantidad =====
    //$('#txtCantidad_add, #txtCantidad_edit').on('keydown', function (event) {
    //    const allowed = [8, 9, 13, 27, 46]; // Backspace, Tab, Enter, Escape, Delete
    //    const arrows = [37, 38, 39, 40];    // Flecha izquierda, arriba, derecha, abajo

    //    if (allowed.includes(event.keyCode) || arrows.includes(event.keyCode)) {
    //        return true;
    //    }
    //    // Permitir Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    //    if (event.ctrlKey || event.metaKey) {
    //        return true;
    //    }
    //    // Solo dígitos (0-9)
    //    if (event.keyCode < 48 || event.keyCode > 57) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

    // ===== VALIDACIÓN EN TIEMPO REAL =====
    function validarCampo(input, mensaje = 'Este campo es requerido') {
        const el = $(input);
        const valor = el.val().trim();

        if (valor === '') {
            el.addClass('is-invalid');
            if (el.next('.invalid-feedback').length === 0) {
                el.after(`<div class="invalid-feedback">${mensaje}</div>`);
            }
            return false;
        } else {
            el.removeClass('is-invalid');
            el.next('.invalid-feedback').remove();
            return true;
        }
    }

    function validarCantidad(input) {
        const el = $(input);
        const valor = el.val().trim();

        if (valor === '' || isNaN(parseInt(valor)) || parseInt(valor) < 1 || valor.includes('.')) {
            el.addClass('is-invalid');
            if (el.next('.invalid-feedback').length === 0) {
                el.after('<div class="invalid-feedback">Debe ser un número entero mayor o igual a 1</div>');
            }
            return false;
        } else {
            el.removeClass('is-invalid');
            el.next('.invalid-feedback').remove();
            return true;
        }
    }

    // Limpiar validaciones al escribir
    $('#txtCod_add, #txtDsc_add, #txtCantidad_add').on('input', function () {
        if ($(this).val().trim() !== '') {
            $(this).removeClass('is-invalid');
            $(this).next('.invalid-feedback').remove();
        }
    });

    // ===== VALIDAR AGREGAR =====
    function ValidarAdd(obj) {
        let valid = true;

        if (!validarCampo('#txtCod_add', 'El código es requerido')) {
            valid = false;
        }

        if (!validarCampo('#txtDsc_add', 'La descripción es requerida')) {
            valid = false;
        }

        if (!validarCantidad('#txtCantidad_add')) {
            valid = false;
        }

        return valid;
    }

    // ===== LIMPIAR AL CERRAR MODAL AGREGAR =====
    $('#Add_Modal').on('hidden.bs.modal', function () {
        document.getElementById('txtCod_add').value = '';
        document.getElementById('txtDsc_add').value = '';
        document.getElementById('txtCantidad_add').value = '';
        $('#txtCod_add, #txtDsc_add, #txtCantidad_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });

    // ===== BOTÓN AGREGAR =====
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.vchCodUniMed = document.getElementById('txtCod_add').value.trim();
        obj.vchDesUniMed = document.getElementById('txtDsc_add').value.trim();
        obj.IntCantidad = document.getElementById('txtCantidad_add').value.trim();

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');

            let _url = "CreateUnidadMedida";
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
                            title: '¡Unidad de medida creada!',
                            text: 'La unidad de medida se ha registrado correctamente.',
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
                        confirmButtonColor: '#5d87ff',
                        confirmButtonText: 'Entendido'
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
    $('#Edit_Modal').modal('show');
    let _url = "GetUnidadMedida";
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

            document.getElementById('txtCod_edit').value = objResponse.vchCodUniMed;
            document.getElementById('txtDsc_edit').value = objResponse.vchDesUniMed;
            document.getElementById('txtCantidad_edit').value = objResponse.IntCantidad;
            document.getElementById('chkActivo_edit').checked = objResponse.IntEstado;

            // Limpiar validaciones previas
            $('#txtCod_edit, #txtDsc_edit, #txtCantidad_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudo cargar la información de la unidad de medida.',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Entendido'
            });
        }
    });
}

// Validaciones para edición
function validarCampoEdit(input, mensaje = 'Este campo es requerido') {
    const el = $(input);
    const valor = el.val().trim();

    if (valor === '') {
        el.addClass('is-invalid');
        if (el.next('.invalid-feedback').length === 0) {
            el.after(`<div class="invalid-feedback">${mensaje}</div>`);
        }
        return false;
    } else {
        el.removeClass('is-invalid');
        el.next('.invalid-feedback').remove();
        return true;
    }
}

function validarCantidadEdit(input) {
    const el = $(input);
    const valor = el.val().trim();

    if (valor === '' || isNaN(parseInt(valor)) || parseInt(valor) < 1 || valor.includes('.')) {
        el.addClass('is-invalid');
        if (el.next('.invalid-feedback').length === 0) {
            el.after('<div class="invalid-feedback">Debe ser un número entero mayor o igual a 1</div>');
        }
        return false;
    } else {
        el.removeClass('is-invalid');
        el.next('.invalid-feedback').remove();
        return true;
    }
}

function ValidarEdit(obj) {
    let valid = true;

    if (!validarCampoEdit('#txtDsc_edit', 'La descripción es requerida')) {
        valid = false;
    }

    if (!validarCantidadEdit('#txtCantidad_edit')) {
        valid = false;
    }

    return valid;
}

// Limpiar validaciones al escribir en edición
$('#txtDsc_edit, #txtCantidad_edit').on('input', function () {
    if ($(this).val().trim() !== '') {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    }
});

// Limpiar validaciones al cerrar modal de edición
$('#Edit_Modal').on('hidden.bs.modal', function () {
    $('#txtCod_edit, #txtDsc_edit, #txtCantidad_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.IntUniMed = IDTemp;
    obj.vchDesUniMed = document.getElementById('txtDsc_edit').value.trim();
    obj.IntCantidad = document.getElementById('txtCantidad_edit').value.trim();
    obj.IntEstado = document.getElementById('chkActivo_edit').checked;

    let _url = "UpdateUnidadMedida";
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

                    // Actualizar tabla
                    ActualizarTabla();

                    // Mostrar éxito
                    Swal.fire({
                        icon: 'success',
                        title: '¡Unidad de medida actualizada!',
                        text: 'Los cambios han sido guardados correctamente.',
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
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Entendido'
                });
            },
            complete: function () {
                btnSave.prop('disabled', false).html('<i class="ti ti-device-floppy me-1"></i>Guardar');
            }
        });
    }
}