/* OBTIENE LOS DATOS DE LA EMPRESA  */
$.ajax({
    url: 'GetEmpresa',
    type: 'POST',
    success: function (response) {
        $('#txt_nom_Empresa').val(response.DSC_EMPRESA);
        $('#txt_RUC').val(response.RUC_EMPRESA);
        $('#txt_drc_Empresa').val(response.DIR_EMPRESA);
        $('#txt_tlf').val(response.TLF_EMPRESA);
        $('#txt_rz_social').val(response.RAZONSOCIAL_EMPRESA);
    },
    error: function () {
        console.error('Error en cargar el correo');
    }
});

/* OBTIENE LA CONFIGURACION CORREO DE LA EMPRESA  */
$.ajax({
    url: 'GetCorreo',
    type: 'POST',
    success: function (response) {
        $('#txt_serv_Correo').val(response.SERVIDOR);
        $('#txt_port_Correo').val(response.PUERTO);
        $('#txt_remitente_Correo').val(response.REMITENTE);
        $('#txt_prio_Correo').val(response.PRIORIDAD);
        if (response.FLG_AUTENTICACION) { $('#chkAuth_Correo').prop('checked', true); } else { $('#chkAuth_Correo').prop('checked', false); }
        if (response.FLG_HABILITAR) { $('#chkSSL_Correo').prop('checked', true); } else { $('#chkSSL_Correo').prop('checked', false); }

        $('#txt_usuario_Correo').val(response.USUARIO);
        $('#txt_pass_Correo').val(response.CONTRASEÑA);
        disabledAuth();
    },
    error: function () {
        console.error('Error en cargar el correo');
    }
});

/* GUARDAR LOS CAMBIOS REALIZADOS  */
const formConfigEmpresa = document.querySelector("#configEmpresa");
const btnGuardarCambios = document.querySelector("#btn_Save_DatosEmpresa");

btnGuardarCambios.addEventListener("click", () => {

    // validar inputs
    if (!validarFormulario(formConfigEmpresa)) {
        return;
    }


    Promise.all([saveEmpresa(), saveCorreo()])
        .then(([resEmpresa, resCorreo]) => {
            if (!resEmpresa.HUBO_ERROR && !resCorreo.HUBO_ERROR) {
                /*console.log("Ambos guardados con exito");*/
                alertaExito();
            }
            else {
               /* console.log("Hubo un error en alguno de los guardados");*/
                alertaErrorBackend();
            }
        })
        .catch(error => {
            /*console.error("Error en una de las solicitudes:", error);*/
            alertaErrorInesperado(error);
        }); 
});

function validarFormulario(form) {
    let esValido = true;
    const inputsConfigEmpresa = form.querySelectorAll("input[type='text'], select");

    inputsConfigEmpresa.forEach(input => {
        input.classList.remove("is-invalid", "is-valid");

        if (input.value.trim() === "") {
            input.value = "";
            input.classList.add("is-invalid");
            esValido = false;
        } else {
            input.classList.add("is-valid");
        }
    });

    return esValido;
}

/* VALIDAR INPUTS AL ESCRIBIR */
const inputs = document.querySelectorAll("#configEmpresa input[type='text']")
inputs.forEach(input => {
    input.addEventListener("input", () => {
        if (input.value.trim() === "") {
            input.classList.remove("is-valid");
            input.classList.add("is-invalid");
        } else {
            input.classList.add("is-valid");
            input.classList.remove("is-invalid");
        }
    });
});

function saveEmpresa() {
    //return new Promise((resolve) => {
    //    resolve({ HUBO_ERROR: true });
    //});
    return $.ajax({
        url: 'SaveEmpresa',
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        data: JSON.stringify({
            DSC_EMPRESA: $('#txt_nom_Empresa').val(),
            RUC_EMPRESA: $('#txt_RUC').val(),
            DIR_EMPRESA: $('#txt_drc_Empresa').val(),
            TLF_EMPRESA: $('#txt_tlf').val(),
            RAZONSOCIAL_EMPRESA: $('#txt_rz_social').val()
        }),
        dataType:"json"
    });
}

function saveCorreo() {
    return $.ajax({
        url: 'SaveCorreo',
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        data: JSON.stringify({
            SERVIDOR: $('#txt_serv_Correo').val(),
            PUERTO: $('#txt_port_Correo').val(),
            REMITENTE: $('#txt_remitente_Correo').val(),
            PRIORIDAD: $('#txt_prio_Correo').val(),
            FLG_AUTENTICACION: document.getElementById('chkAuth_Correo').checked,
            FLG_HABILITAR: document.getElementById('chkSSL_Correo').checked,
            USUARIO: $('#txt_usuario_Correo').val(),
            CONTRASEÑA: $('#txt_pass_Correo').val()
        }),
        dataType: "json"
    });
}

$('#chkAuth_Correo').change(function () {
    disabledAuth();
});

function disabledAuth() {
    var authchk = document.getElementById('chkAuth_Correo').checked;
    if (!authchk) {
        $('#txt_usuario_Correo').prop("disabled", true);
        $('#txt_pass_Correo').prop("disabled", true);
    } else {
        $('#txt_usuario_Correo').prop("disabled", false);
        $('#txt_pass_Correo').prop("disabled", false);
    }
}

/* ALERTAS */
function alertaExito() {
    Swal.fire({
        title: "Cambios guardados",
        text: "La información fue registrada correctamente",
        icon: "success"
    });
}

function alertaErrorBackend() {
    Swal.fire({
        title: "No se pudo guardar",
        text: "El servidor rechazó la información enviada. Revise los datos e inténtelo nuevamente.",
        icon: "warning"
    });
}

function alertaErrorInesperado(error) {
    Swal.fire({
        title: "Error inesperado",
        text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
        icon: "error",
        footer: `<small>Detalles: ${error}</small>`
    });
}



//document.getElementById('btn_Save_DatosEmpresa').addEventListener('click', ()=> {

//    // GUARDA LOS CAMBIOS A DATOS DE EMPRESA
//    var obj = new Object();
//    obj.DSC_EMPRESA = $('#txt_nom_Empresa').val();
//    obj.RUC_EMPRESA = $('#txt_RUC').val();
//    obj.DIR_EMPRESA = $('#txt_drc_Empresa').val();
//    obj.TLF_EMPRESA = $('#txt_tlf').val();
//    obj.RAZONSOCIAL_EMPRESA = $('#txt_rz_social').val();

//    $.ajax({
//        url: 'SaveEmpresa',
//        contentType: "application/json; charset=utf-8",
//        type: 'POST',
//        data: JSON.stringify(obj),
//        dataType: "json",

//        success: function (response) {
//            if (!response.HUBO_ERROR) {
//                console.log('Datos de empresa guardados exitosamente');
//                // Feedback EXITO
//            } else {
//                console.error('Error al guardar la información');
//                // Feedback ERROR ENVIAR
//            }

//        },
//        error: function () {
//            console.error('Error en la solicitud AJAX');
//            // Feedback ERROR RECIBIR
//        }
//    });
//});

/* CONFIGURACION DE CORREO */



//document.getElementById('btn_Save_Correo').addEventListener('click', function (e) {

//    var obj = new Object();
//    obj.SERVIDOR = $('#txt_serv_Correo').val();
//    obj.PUERTO = $('#txt_port_Correo').val();
//    obj.REMITENTE = $('#txt_remitente_Correo').val();
//    obj.PRIORIDAD = $('#txt_prio_Correo').val();
//    obj.FLG_AUTENTICACION = document.getElementById('chkAuth_Correo').checked; //$('#chkAuth_Correo').val();
//    obj.FLG_HABILITAR = document.getElementById('chkSSL_Correo').checked;//$('#chkSSL_Correo').val();
//    obj.USUARIO = $('#txt_usuario_Correo').val();
//    obj.CONTRASEÑA = $('#txt_pass_Correo').val();


//    $.ajax({
//        url: 'SaveCorreo',
//        type: 'POST',
//        data: JSON.stringify(obj),
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",

//        success: function (response) {
//            if (!response.HUBO_ERROR) {
//                console.log('Datos del correo guardados exitosamente');
//                // Feedback EXITO
//            } else {
//                console.error('Error al guardar la información');
//                // Feedback ERROR ENVIAR
//            }

//        },
//        error: function () {
//            console.error('Error en la solicitud AJAX');
//            // Feedback ERROR RECIBIR
//        }
//    });
//});
