$(document).ready(function () {
    load();

    function load() {
        ValidarNumAlmacenes();
    }

    function ValidarNumAlmacenes() {
        var DivcbxUbicacion_add = $('#Div_cbxUbicacion_add');
        var txtUbicacion_add = $('#txtUbicacion_add');
        var cbxAlmacen_add = document.getElementById("cbxUbicacion_add");
        var count_cbxAlmacen_add = cbxAlmacen_add.options.length;

        DivcbxUbicacion_add.css('display', 'none');
        txtUbicacion_add.css('display', 'none');

        if (count_cbxAlmacen_add > 0) {
            DivcbxUbicacion_add.css('display', 'block');
        } else {
            txtUbicacion_add.css('display', 'block');
        }
    }

    // ===== VALIDACIÓN EN TIEMPO REAL =====
    function validarInput(inputElement, valor, mensaje = 'Este campo es requerido') {
        const el = $(inputElement);
        const val = (typeof valor !== 'undefined') ? valor : el.val();
        const valTrim = (typeof val === 'string') ? val.trim() : val;

        if (valTrim === '' || valTrim === null || valTrim === undefined || (Array.isArray(valTrim) && valTrim.length === 0)) {
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

    // Limpiar validaciones al escribir
    $('#txtCodUbicacion_add, #txtDscUbicacion_add').on('input', function () {
        if ($(this).val().trim() !== '') {
            $(this).removeClass('is-invalid');
            $(this).next('.invalid-feedback').remove();
        }
    });

    $('#cbxUbicacion_add').on('change', function () {
        if ($(this).val() && $(this).val().length > 0) {
            $(this).removeClass('is-invalid');
            $(this).next('.invalid-feedback').remove();
        }
    });

    // ===== VALIDAR AGREGAR =====
    function ValidarAdd(obj) {
        let valid = true;

        if (!validarInput('#txtCodUbicacion_add', obj.codUbicacion, 'El código de ubicación es requerido')) {
            valid = false;
        }

        if (!validarInput('#txtDscUbicacion_add', obj.dscUbicacion, 'La descripción de ubicación es requerida')) {
            valid = false;
        }

        if (!validarInput('#cbxUbicacion_add', obj.ListidAlmacen, 'Debe seleccionar al menos un almacén')) {
            valid = false;
        }

        return valid;
    }

    // ===== LIMPIAR AL CERRAR MODAL AGREGAR =====
    $('#Add_ubicacion').on('hidden.bs.modal', function () {
        document.getElementById('txtCodUbicacion_add').value = '';
        document.getElementById('txtDscUbicacion_add').value = '';
        $('#cbxUbicacion_add').val(null).trigger('change');

        $('#txtCodUbicacion_add, #txtDscUbicacion_add, #cbxUbicacion_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();
    });

    // ===== BOTÓN AGREGAR =====
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.codUbicacion = document.getElementById('txtCodUbicacion_add').value.trim();
        obj.dscUbicacion = document.getElementById('txtDscUbicacion_add').value.trim();
        obj.ListidAlmacen = $('#cbxUbicacion_add').val();

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Creando...');

            let _url = "CreateUbicacion";
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
                            title: 'Error al crear',
                            text: response.MENSAJE_ERROR,
                            confirmButtonColor: '#5d87ff',
                            confirmButtonText: 'Entendido'
                        });
                    } else {
                        // Cerrar modal
                        $('#Add_ubicacion').modal('hide');

                        // Mostrar éxito
                        setTimeout(() => {
                            Swal.fire({
                                icon: 'success',
                                title: '¡Ubicación creada exitosamente!',
                                text: 'La ubicación ha sido registrada correctamente.',
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
    $('#Edit_ubicacion').modal('show');
    let _url = "GetUbicacion";
    var obj = new Object();
    obj.CodUbicacion = id;

    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        success: function (response) {
            var objResponse = response.Entity;

            document.getElementById('txtCodUbicacion_edit').value = objResponse[0].vchCod_Ubicacion;
            document.getElementById('txtDscUbicacion_edit').value = objResponse[0].vchDSC_Ubicacion;

            if (objResponse[0].IdAlmacen == -1) {
                document.getElementById('cbxUbicacion_edit').selectedIndex = 0;
            } else {
                $('#cbxUbicacion_edit').val(objResponse.map(obj => obj.intIdAlmacen));
                $('#cbxUbicacion_edit').trigger('change');
            }

            document.getElementById('chkActivo_edit').checked = objResponse[0].intActivo;

            // Limpiar validaciones previas
            $('#txtCodUbicacion_edit, #txtDscUbicacion_edit, #cbxUbicacion_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudo cargar la información de la ubicación.',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Entendido'
            });
        }
    });
}

// Validación reutilizada en edición (misma función del scope global en producto)
function validarCampoEdit(inputElement, valor, mensaje = 'Este campo es requerido') {
    const el = $(inputElement);
    const valTrim = (typeof valor === 'string') ? valor.trim() : valor;

    if (valTrim === '' || valTrim === null || valTrim === undefined || (Array.isArray(valTrim) && valTrim.length === 0)) {
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

function ValidarEdit(obj) {
    let valid = true;

    if (!validarCampoEdit('#txtCodUbicacion_edit', obj.codUbicacion, 'El código de ubicación es requerido')) {
        valid = false;
    }

    if (!validarCampoEdit('#txtDscUbicacion_edit', obj.DSCUbicacion, 'La descripción de ubicación es requerida')) {
        valid = false;
    }

    // Solo validar almacén si está activo
    if (obj.activo == true) {
        if (!validarCampoEdit('#cbxUbicacion_edit', obj.ListidAlmacen, 'La ubicación activa requiere al menos un almacén')) {
            valid = false;
        }
    }

    return valid;
}

// Limpiar validaciones al escribir en edición
$('#txtCodUbicacion_edit, #txtDscUbicacion_edit').on('input', function () {
    if ($(this).val().trim() !== '') {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    }
});

$('#cbxUbicacion_edit').on('change', function () {
    if ($(this).val() && $(this).val().length > 0) {
        $(this).removeClass('is-invalid');
        $(this).next('.invalid-feedback').remove();
    }
});

// Limpiar validaciones al cerrar modal de edición
$('#Edit_ubicacion').on('hidden.bs.modal', function () {
    $('#txtCodUbicacion_edit, #txtDscUbicacion_edit, #cbxUbicacion_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.codUbicacion = IDTemp;
    obj.codUbicacionEdit = document.getElementById('txtCodUbicacion_edit').value.trim();
    obj.DSCUbicacion = document.getElementById('txtDscUbicacion_edit').value.trim();
    obj.ListidAlmacen = $('#cbxUbicacion_edit').val();
    obj.activo = document.getElementById('chkActivo_edit').checked;

    let _url = "UpdateUbicacion";
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
                    $('#Edit_ubicacion').modal('hide');

                    // Mostrar éxito
                    setTimeout(() => {
                        Swal.fire({
                            icon: 'success',
                            title: '¡Ubicación actualizada exitosamente!',
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