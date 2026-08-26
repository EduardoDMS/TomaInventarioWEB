$(document).ready(function () {
    load();

    function load() {
        combosInsertUser();
    }

    // ===== TOGGLE PASSWORD (reutilizable) =====
    function togglePassword(inputId, buttonId) {
        $('#' + buttonId).on('click', function () {
            const passInput = $('#' + inputId);
            const icon = $(this).find('i');

            if (passInput.attr('type') === 'password') {
                passInput.attr('type', 'text');
                icon.removeClass('ti-eye').addClass('ti-eye-off');
            } else {
                passInput.attr('type', 'password');
                icon.removeClass('ti-eye-off').addClass('ti-eye');
            }
        });
    }

    togglePassword('txtPass_add', 'togglePassAdd');
    togglePassword('txtPass_edit', 'togglePassEdit');

    // ===== VALIDAR AGREGAR =====
    function ValidarAdd(obj) {
        let valid = true;
        console.log(obj);

        //if (!validarCampo('#txtUsuario_add', 'El nombre de usuario es requerido')) {
        //    valid = false;
        //}

        if (!validarCampo('#txtPass_add')) {
            valid = false;
        }

        if (!validarCampo('#NombreInput')) {
            valid = false;
        }

        if (!validarCampo('#ApellidoInput')) {
            valid = false;
        }


        if (obj.perfil === 'OPE') {
            if (!validarCampo('#cbxAlmacen_add')) {
                valid = false;
            }
        }

        return valid;
    }

    // ===== VALIDACIÓN INLINE =====
    function validarCampo(input) {
        //const el = $(input);
        //const valor = el.val() ? el.val().trim() : '';
        //console.log(valor);

        //if (valor === '' || (Array.isArray(valor) && valor.length === 0)) {
        //    el.addClass('is-invalid');
        //    if (el.next('.invalid-feedback').length === 0) {
        //        el.after(`<div class="invalid-feedback">${mensaje}</div>`);
        //    }
        //    return false;
        //} else {
        //    el.removeClass('is-invalid');
        //    el.next('.invalid-feedback').remove();
        //    return true;
        //}

        const el = $(input);
        const valor = el.val();
        console.log(el);
        let esValido = false;

        if (Array.isArray(valor)) {
            esValido = valor.length > 0;
        } else if (valor === null || valor === undefined) {
            esValido = false;
        } else {
            esValido = valor.toString().trim() !== '';
        }

        if (!esValido) {
            el.addClass('is-invalid');

            //if (el.next('.invalid-feedback').length === 0) {
            //    el.after(`<div class="invalid-feedback">${mensaje}</div>`);
            //}

            return false;
        } else {
            el.removeClass('is-invalid');
            /*el.next('.invalid-feedback').remove();*/
            return true;
        }
    }

    // Limpiar validaciones al escribir
    $('#txtUsuario_add, #txtPass_add,#NombreInput,#ApellidoInput, #txtUsuario_edit, #txtPass_edit').on('input', function () {
        if ($(this).val().trim() !== '') {
            $(this).removeClass('is-invalid');
            /*$(this).next('.invalid-feedback').remove();*/
        }
    });

    $('#cbxAlmacen_add, #cbxAlmacen_edit').on('change', function () {
        if ($(this).val() && $(this).val().length > 0) {
            $(this).removeClass('is-invalid');
            $(this).next('.invalid-feedback').remove();
        }
    });

    // ===== COMBOS INSERTAR =====
    function combosInsertUser() {
        var DivcbxAlmacen_add = $('#Div_cbxAlmacen_add');
        var txtAlmacen_add = $('#txtAlmacen_add');
        var count_cbxAlmacen_add = document.getElementById("cbxAlmacen_add").options.length;

        $('#cbxPerfil_add').on('change', function () {
            DivcbxAlmacen_add.addClass('d-none');
            txtAlmacen_add.addClass('d-none');
            $('#cbxAlmacen_add').removeClass('is-invalid');
            $('#cbxAlmacen_add').next('.invalid-feedback').remove();

            if ($(this).val() === 'OPE') {
                if (count_cbxAlmacen_add > 0) {
                    DivcbxAlmacen_add.removeClass('d-none');
                } else {
                    txtAlmacen_add.removeClass('d-none');
                }
            }
        });
    }

    // ===== COMBOS EDITAR ===== document
    function combosEditUser() {
        var DivcbxAlmacen_edit = $('#Div_cbxAlmacen_edit');
        var txtAlmacen_edit = $('#txtAlmacen_edit');
        var count_cbxAlmacen_edit = document.getElementById("cbxAlmacen_edit").options.length;

        // Mostrar/ocultar según el valor actual al cargar
        DivcbxAlmacen_edit.addClass('d-none');
        txtAlmacen_edit.addClass('d-none');

        if ($('#cbxPerfil_edit').val() === 'OPE') {
            if (count_cbxAlmacen_edit > 0) {
                DivcbxAlmacen_edit.removeClass('d-none');
            } else {
                txtAlmacen_edit.removeClass('d-none');
            }
        }

        // Rebind el evento change (por si se llama múltiples veces al editar)
        $('#cbxPerfil_edit').off('change').on('change', function () {
            DivcbxAlmacen_edit.addClass('d-none');
            txtAlmacen_edit.addClass('d-none');
            $('#cbxAlmacen_edit').removeClass('is-invalid');
            $('#cbxAlmacen_edit').next('.invalid-feedback').remove();

            if ($(this).val() === 'OPE') {
                if (count_cbxAlmacen_edit > 0) {
                    DivcbxAlmacen_edit.removeClass('d-none');
                    $('#cbxAlmacen_edit').val(null).trigger('change');
                } else {
                    txtAlmacen_edit.removeClass('d-none');
                }
            }
        });
    }


    // ===== LIMPIAR CAMPOS AL CERRAR MODAL AGREGAR =====
    $('#Add_usuario').on('hidden.bs.modal', function () {
        document.getElementById('NombreInput').value = '';
        document.getElementById('ApellidoInput').value = '';
        document.getElementById('txtUsuario_add').value = '';
        document.getElementById('txtPass_add').value = '';
        document.getElementById('txtPass_add').setAttribute('type', 'password');
        document.getElementById('cbxPerfil_add').selectedIndex = 0;
        document.getElementById('cbxAlmacen_add').selectedIndex = 0;

        $('#Div_cbxAlmacen_add').addClass('d-none');
        $('#txtAlmacen_add').addClass('d-none');

        $('#txtUsuario_add, #txtPass_add, #cbxAlmacen_add').removeClass('is-invalid');
        $('.invalid-feedback').remove();

        // Resetear ícono del toggle de password
        $('#togglePassAdd i').removeClass('ti-eye-off').addClass('ti-eye');
    });

    // ===== GENERAR USUARIO CREAR =====
    var nombreInput = document.getElementById('NombreInput');
    var apellidoInput = document.getElementById('ApellidoInput');
    var usuarioGenerado = document.getElementById('txtUsuario_add');

    function GenerarUsuario() {
        let nombre = nombreInput.value.trim().replace(/\s+/g, '');
        let apellidoCompleto = apellidoInput.value.trim();
        let primerApellido = apellidoCompleto.split(' ')[0];

        if (nombre.length > 0 || apellidoCompleto.length > 0) {
            let usuario = nombre.charAt(0).toUpperCase() + nombre.charAt(1).toLowerCase() + primerApellido.toLowerCase();

            usuarioGenerado.value = usuario;
        } else {
            usuarioGenerado.value = "";
        }
    }

    nombreInput.addEventListener("input", GenerarUsuario);
    apellidoInput.addEventListener("input", GenerarUsuario);

    // ===== GENERAR USUARIO EDITAR =====
    var nombreInput_edit = document.getElementById('txtUsuario_nombre');
    var apellidoInput_edit = document.getElementById('txtUsuario_apellido');
    var usuarioGenerado_edit = document.getElementById('txtUsuario_edit');

    function GenerarUsuario_edit() {
        let nombre = nombreInput_edit.value.trim().replace(/\s+/g, '');
        let apellidoCompleto = apellidoInput_edit.value.trim();
        let primerApellido = apellidoCompleto.split(' ')[0];

        if (nombre.length > 0 || apellidoCompleto.length > 0) {
            let usuario = nombre.charAt(0).toUpperCase() + nombre.charAt(1).toLowerCase() + primerApellido.toLowerCase();

            usuarioGenerado_edit.value = usuario;
        } else {
            usuarioGenerado_edit.value = "";
        }
    }

    nombreInput_edit.addEventListener("input", GenerarUsuario_edit);
    apellidoInput_edit.addEventListener("input", GenerarUsuario_edit);

    // ===== VALIDAR INPUTS SOLO LETRAS =====
    document.querySelectorAll('.onlyText').forEach(campo => {

        campo.addEventListener('input', function () {

            this.value = this.value.replace(
                /[^a-zA-ZáéíóúÁÉÍÓÚñÑ\s]/g,
                ''
            );

        });

    });

    // ===== BOTÓN AGREGAR =====
    document.getElementById('btn_Add').addEventListener('click', function (e) {
        var obj = new Object();
        obj.nombre = document.getElementById('NombreInput').value.trim(); // se agrego este campo
        obj.apellido = document.getElementById('ApellidoInput').value.trim(); // se agrego este campo
        obj.cod_usuario = document.getElementById('txtUsuario_add').value.trim();
        obj.clave = document.getElementById('txtPass_add').value.trim();
        obj.perfil = document.getElementById('cbxPerfil_add').value;
        obj.idAlmacen = $('#cbxAlmacen_add').val();

        var _valid = ValidarAdd(obj);

        if (_valid) {
            const btnAdd = $('#btn_Add');
            btnAdd.prop('disabled', true).html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Guardando...');
            // cambiar para que el front end resiva nombre y apellido 
            let _url = "CreateUsuario";
            
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: _url,
                data: JSON.stringify(obj),
                success: function (response) {
                    //console.log("Payload enviado:", obj);
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
                        $('#Add_usuario').modal('hide');

                        // Actualizar tabla
                        /*ActualizarTabla();*/

                        // Mostrar éxito
                        setTimeout(() => {
                            Swal.fire({
                                icon: 'success',
                                title: '¡Usuario creado exitosamente!',
                                text: 'El usuario ha sido registrado correctamente.',
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
});

// ==================== EDICIÓN ====================
var IDTemp = -1;

function editar(id) {
    $('#Edit_usuario').modal('show');

    let _url = "GetUsuario";
    var obj = new Object();
    obj.idUsuario = id;

    $.ajax({
        type: "Post",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: _url,
        data: JSON.stringify(obj),
        success: function (response) {
            var objUsuario = response.Entity;
            //console.log(response);

            document.getElementById('txtUsuario_nombre').value = objUsuario[0].Nombre;
            document.getElementById('txtUsuario_apellido').value = objUsuario[0].Apellido;
            document.getElementById('txtUsuario_edit').value = objUsuario[0].Usuario;
            document.getElementById('txtPass_edit').value = objUsuario[0].Contraseña;
            document.getElementById('cbxPerfil_edit').value = objUsuario[0].Perfil;
            combosEditUser();

            if (objUsuario[0].IdAlmacen == -1) {
                document.getElementById('cbxAlmacen_edit').selectedIndex = 0;
            } else {
                $('#cbxAlmacen_edit').val(objUsuario.map(u => u.IdAlmacen));
                $('#cbxAlmacen_edit').trigger('change');
            }

            document.getElementById('chkActivo_edit').checked = objUsuario[0].Activo;

            // Limpiar validaciones previas
            $('#txtUsuario_edit, #txtPass_edit, #cbxAlmacen_edit').removeClass('is-invalid');
            $('.invalid-feedback').remove();

            IDTemp = id;
        },
        error: function (result) {
            Swal.fire({
                icon: 'error',
                title: 'Error al cargar',
                text: 'No se pudieron cargar los datos del usuario.',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Entendido'
            });
        }
    });
}

function combosEditUser() {
    var DivcbxAlmacen_edit = $('#Div_cbxAlmacen_edit');
    var txtAlmacen_edit = $('#txtAlmacen_edit');
    var count_cbxAlmacen_edit = document.getElementById("cbxAlmacen_edit").options.length;

    DivcbxAlmacen_edit.addClass('d-none');
    txtAlmacen_edit.addClass('d-none');

    if ($('#cbxPerfil_edit').val() === 'OPE') {
        if (count_cbxAlmacen_edit > 0) {
            DivcbxAlmacen_edit.removeClass('d-none');
        } else {
            txtAlmacen_edit.removeClass('d-none');
        }
    }

    $('#cbxPerfil_edit').off('change').on('change', function () {
        DivcbxAlmacen_edit.addClass('d-none');
        txtAlmacen_edit.addClass('d-none');
        $('#cbxAlmacen_edit').removeClass('is-invalid');
        $('#cbxAlmacen_edit').next('.invalid-feedback').remove();

        if ($(this).val() === 'OPE') {
            if (count_cbxAlmacen_edit > 0) {
                DivcbxAlmacen_edit.removeClass('d-none');
                $('#cbxAlmacen_edit').val(null).trigger('change');
            } else {
                txtAlmacen_edit.removeClass('d-none');
            }
        }
    });
}

// ===== VALIDAR EDICIÓN =====
function ValidarEdit(obj) {
    let valid = true;

    const validarCampoEdit = function (input, mensaje) {
        //const el = $(input);
        //const valor = el.val() ? el.val().trim() : '';

        //if (valor === '' || (Array.isArray(valor) && valor.length === 0)) {
        //    el.addClass('is-invalid');
        //    if (el.next('.invalid-feedback').length === 0) {
        //        el.after(`<div class="invalid-feedback">${mensaje}</div>`);
        //    }
        //    return false;
        //} else {
        //    el.removeClass('is-invalid');
        //    el.next('.invalid-feedback').remove();
        //    return true;
        //}
        const el = $(input);
        const valor = el.val();
        //console.log(valor);
        let esValido = false;

        if (Array.isArray(valor)) {
            esValido = valor.length > 0;
        } else if (valor === null || valor === undefined) {
            esValido = false;
        } else {
            esValido = valor.toString().trim() !== '';
        }

        if (!esValido) {
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
    };

    if (!validarCampoEdit('#txtUsuario_nombre', 'El nombre es requerido')) {
        valid = false;
    }

    if (!validarCampoEdit('#txtUsuario_apellido', 'El apellido es requerido')) {
        valid = false;
    }

    if (!validarCampoEdit('#txtUsuario_edit', 'El usuario es requerido')) {
        valid = false;
    }

    if (!validarCampoEdit('#txtPass_edit', 'La contraseña es requerida')) {
        valid = false;
    }

    // Solo validar almacén si el perfil es operador
    if (obj.perfil === 'OPE') {
        if (!validarCampoEdit('#cbxAlmacen_edit', 'Debe seleccionar un almacén para operador')) {
            valid = false;
        }
    }

    return valid;
}

// ===== LIMPIAR AL CERRAR MODAL EDITAR =====
$('#Edit_usuario').on('hidden.bs.modal', function () {
    document.getElementById('txtPass_edit').setAttribute('type', 'password');
    $('#txtUsuario_edit, #txtPass_edit, #cbxAlmacen_edit').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    $('#togglePassEdit i').removeClass('ti-eye-off').addClass('ti-eye');
});

$("#btn_SaveEdit").on("click", function () {
    saveEdit();
});

function saveEdit() {
    var obj = new Object();
    obj.idUsuario = IDTemp;
    obj.nombreUsuario = document.getElementById('txtUsuario_nombre').value.trim();
    obj.apellidoUsuario = document.getElementById('txtUsuario_apellido').value.trim(); // Apellido
    obj.cod_usuario = document.getElementById('txtUsuario_edit').value.trim(); // cambio aqui <========================================================================
    obj.clave = document.getElementById('txtPass_edit').value.trim();
    obj.perfil = document.getElementById('cbxPerfil_edit').value;
    obj.idAlmacen = $('#cbxAlmacen_edit').val();
    obj.activo = document.getElementById('chkActivo_edit').checked;

    let _url = "UpdateUsuario";
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
                    $('#Edit_usuario').modal('hide');

                    // Mostrar éxito
                    setTimeout(() => {
                        Swal.fire({
                            icon: 'success',
                            title: '¡Usuario actualizado exitosamente!',
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