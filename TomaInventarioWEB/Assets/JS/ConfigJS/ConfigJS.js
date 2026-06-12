//Imagen  Banner
const croppie = new Croppie(document.getElementById('cropped-image'), {
    enableExif: true,
    viewport: { width: 200, height: 104 },
    boundary: { width: 400, height: 350 },
    enableResize: false

});
croppie.setZoom(0.2);
document.getElementById('archivo').addEventListener('change', function (e) {
    const reader = new FileReader();

    reader.onload = function (event) {
        croppie.bind({
            url: event.target.result
        });
    };

    reader.readAsDataURL(e.target.files[0]);
});
document.getElementById('Save_image').addEventListener('click', function (e) {
    const image = document.getElementById('cropped-image');
    const input = document.getElementById('archivo');


    croppie.result('base64').then(function (imagenRecortada) {

        var cleanedBase64String = imagenRecortada.replace(/^data:image\/(png|jpeg|jpg);base64,/, '');
        var obj = new Object();
        $.ajax({
            url: 'GuardarImagenBanner',
            type: 'POST',
            data: { imagen: cleanedBase64String },
            dataType: "json",

            success: function (response) {
                if (response.success) {
                    //Console.log('Imagen guardada exitosamente');
                    $('#txtvar').text('Imagen guardada exitosamente');
                    $('#success_modal').modal('show');
                    $('#success_modal').on('hidden.bs.modal', function () {
                        location.reload();
                    });
                    //location.reload();
                } else {
                    console.error('Error al guardar la imagen');
                    $('#txtvarError').text('Error al guardar la imagen');
                    $('#error_modal').modal('show');
                }

            },
            error: function () {
                console.error('Error en la solicitud AJAX');
            }
        });
    });
});
function eliminarImagen() {
    $.ajax({
        url: 'EliminarImagenBanner',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                //Console.log('Imagen eliminada exitosamente');

                // Actualiza la página o realiza alguna acción después de la eliminación
                $('#txtvar').text('Imagen eliminada exitosamente');
                $('#success_modal').modal('show');

                $('#success_modal').on('hidden.bs.modal', function () {
                    location.reload();
                });
                //location.reload();
            } else {
                console.error('Error al eliminar la imagen');
                $('#txtvarError').text('Error al eliminar la imagen');
                $('#error_modal').modal('show');

            }
        },
        error: function () {
            console.error('Error en la solicitud para eliminar la imagen');
        }
    });
}
$('#Delete_image').click(function () {
    if (confirm('¿Estás seguro de eliminar la imagen?')) {
        eliminarImagen();
    }
});
//Fin Imagen Banner

//=======================================================================

//Imagen  Excel
const croppieExcel = new Croppie(document.getElementById('cropped-image-Excel'), {
    enableExif: true,
    viewport: { width: 250, height: 111 },
    boundary: { width: 400, height: 350 },
    enableResize: false

});
croppieExcel.setZoom(0.2);

document.getElementById('archivoExcel').addEventListener('change', function (e) {
    const reader = new FileReader();

    reader.onload = function (event) {
        croppieExcel.bind({
            url: event.target.result
        });
    };

    reader.readAsDataURL(e.target.files[0]);
});
document.getElementById('Save_image_excel').addEventListener('click', function (e) {
    var archivoInput = document.getElementById('archivoExcel');
    var archivo = archivoInput.files[0];

    var formData = new FormData();
    formData.append('imagen', archivo);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'GuardarImagenExcel', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            //Console.log('Imagen guardada exitosamente');
            $('#txtvar').text('Imagen guardada exitosamente');
            $('#success_modal').modal('show');
            $('#success_modal').on('hidden.bs.modal', function () {
                location.reload();
            });
        } else {
            console.error('Error al guardar la imagen');
            $('#txtvarError').text('Error al guardar la imagen');
            $('#error_modal').modal('show');
        }
    };
    xhr.send(formData);

    const image = document.getElementById('cropped-image-Excel');
    const input = document.getElementById('archivoExcel');
    croppieExcel.result('base64').then(function (imagenRecortada) {

        var cleanedBase64String = imagenRecortada.replace(/^data:image\/(png|jpeg|jpg);base64,/, '');
        var obj = new Object();
        $.ajax({
            url: 'GuardarImagenExcel',
            type: 'POST',
            data: { imagen: cleanedBase64String },
            dataType: "json",

            success: function (response) {
                if (response.success) {
                    //Console.log('Imagen guardada exitosamente');
                    $('#txtvar').text('Imagen guardada exitosamente');
                    $('#success_modal').modal('show');
                    $('#success_modal').on('hidden.bs.modal', function () {
                        location.reload();
                    });
                    //location.reload();
                } else {
                    console.error('Error al guardar la imagen');
                    $('#txtvarError').text('Error al guardar la imagen');
                    $('#error_modal').modal('show');
                }

            },
            error: function () {
                console.error('Error en la solicitud AJAX');
            }
        });
    });
});
function EliminarImagenExcel() {
    $.ajax({
        url: 'EliminarImagenExcel',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                console.log('Imagen eliminada exitosamente');

                // Actualiza la página o realiza alguna acción después de la eliminación
                $('#txtvar').text('Imagen eliminada exitosamente');
                $('#success_modal').modal('show');

                $('#success_modal').on('hidden.bs.modal', function () {
                    location.reload();
                });
                //location.reload();
            } else {
                console.error('Error al eliminar la imagen');
                $('#txtvarError').text('Error al eliminar la imagen');
                $('#error_modal').modal('show');

            }
        },
        error: function () {
            console.error('Error en la solicitud para eliminar la imagen');
        }
    });
}
$('#Delete_image_excel').click(function () {
    if (confirm('¿Estás seguro de eliminar la imagen?')) {
        EliminarImagenExcel();
    }
});
//Fin Imagen Excel

//=======================================================================

//Imagen Fondo
document.getElementById('Save_image_fondo').addEventListener('click', function (e) {

    var archivoInput = document.getElementById('archivoFondo');
    var archivo = archivoInput.files[0];

    var formData = new FormData();
    formData.append('imagen', archivo);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'GuardarImagenFondo', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            //Console.log('Imagen guardada exitosamente');
            $('#txtvar').text('Imagen guardada exitosamente');
            $('#success_modal').modal('show');
            $('#success_modal').on('hidden.bs.modal', function () {
                location.reload();
            });
        } else {
            console.error('Error al guardar la imagen');
            $('#txtvarError').text('Error al guardar la imagen');
            $('#error_modal').modal('show');
        }
    };
    xhr.send(formData);

});
function EliminarImagenFondo() {
    $.ajax({
        url: 'EliminarImagenFondo',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                //Console.log('Imagen eliminada exitosamente');

                // Actualiza la página o realiza alguna acción después de la eliminación
                $('#txtvar').text('Imagen eliminada exitosamente');
                $('#success_modal').modal('show');

                $('#success_modal').on('hidden.bs.modal', function () {
                    location.reload();
                });
                //location.reload();
            } else {
                console.error('Error al eliminar la imagen');
                $('#txtvarError').text('Error al eliminar la imagen');
                $('#error_modal').modal('show');

            }
        },
        error: function () {
            console.error('Error en la solicitud para eliminar la imagen');
        }
    });
}
$('#Delete_image_fondo').click(function () {
    if (confirm('¿Estás seguro de eliminar la imagen?')) {
        EliminarImagenFondo();
    }
});
//Fin Imagen Fondo

//=======================================================================

//EMPRESA

$.ajax({
    url: 'GetEmpresa',
    type: 'POST',
    success: function (response) {
        /*Console.log(response);*/
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

document.getElementById('btn_Save_DatosEmpresa').addEventListener('click', function (e) {

    var obj = new Object();
    obj.DSC_EMPRESA = $('#txt_nom_Empresa').val();
    obj.RUC_EMPRESA = $('#txt_RUC').val();
    obj.DIR_EMPRESA = $('#txt_drc_Empresa').val();
    obj.TLF_EMPRESA = $('#txt_tlf').val();
    obj.RAZONSOCIAL_EMPRESA = $('#txt_rz_social').val();

    $.ajax({
        url: 'SaveEmpresa',
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        data: JSON.stringify(obj),
        dataType: "json",

        success: function (response) {
            if (!response.HUBO_ERROR) {
                //Console.log('Datos de empresa guardados exitosamente');
                $('#txtvar').text('Datos de empresa guardados exitosamente');
                $('#success_modal').modal('show');
                //$('#success_modal').on('hidden.bs.modal', function () {
                    
                //});
                //location.reload();
            } else {
                console.error('Error al guardar la información');
                $('#txtvarError').text('Error al guardar la información');
                $('#error_modal').modal('show');
            }

        },
        error: function () {
            console.error('Error en la solicitud AJAX');
        }
    });
});
//Fin EMPRESA

//=======================================================================

//CORREO

$.ajax({
    url: 'GetCorreo',
    type: 'POST',
    success: function (response) {
        //Console.log(response);
        $('#txt_serv_Correo').val(response.SERVIDOR);
        $('#txt_port_Correo').val(response.PUERTO);
        $('#txt_remitente_Correo').val(response.REMITENTE);
        $('#txt_prio_Correo').val(response.PRIORIDAD) ;
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

document.getElementById('btn_Save_Correo').addEventListener('click', function (e) {

    var obj = new Object();
    obj.SERVIDOR = $('#txt_serv_Correo').val();
    obj.PUERTO = $('#txt_port_Correo').val();
    obj.REMITENTE = $('#txt_remitente_Correo').val();
    obj.PRIORIDAD = $('#txt_prio_Correo').val();
    obj.FLG_AUTENTICACION = document.getElementById('chkAuth_Correo').checked; //$('#chkAuth_Correo').val();
    obj.FLG_HABILITAR = document.getElementById('chkSSL_Correo').checked;//$('#chkSSL_Correo').val();
    obj.USUARIO = $('#txt_usuario_Correo').val();
    obj.CONTRASEÑA = $('#txt_pass_Correo').val();

    
        $.ajax({
            url: 'SaveCorreo',
            type: 'POST',
            data: JSON.stringify(obj),
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (response) {
                if (!response.HUBO_ERROR) {
                    //Console.log('Datos del correo guardados exitosamente');
                    $('#txtvar').text('Datos del correo guardados exitosamente');
                    $('#success_modal').modal('show');
                    //$('#success_modal').on('hidden.bs.modal', function () {
                    //});
                    //location.reload();
                } else {
                    console.error('Error al guardar la información');
                    $('#txtvarError').text('Error al guardar la información');
                    $('#error_modal').modal('show');
                }

            },
            error: function () {
                console.error('Error en la solicitud AJAX');
            }
        });
});
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

//Fin CORREO

//=======================================================================

