function downTemplateCosto() {
    window.location.href = urlDescargarCostos;
}

jQuery(document).ready(function () {

    $('#btnTemplateCosto').click(function () {
        downTemplateCosto();
    });

    $('#ImportxProdxCosto_Modal').on('hidden.bs.modal', function () {
        $("#fileInputCosto").val("");
    });

    $('#btn_ImportCosto').click(function () {
        var fileInput = $("#fileInputCosto")[0];

        if (!fileInput.files || fileInput.files.length === 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Archivo no seleccionado',
                text: 'Por favor, seleccione un archivo para importar.',
                confirmButtonColor: '#5d87ff',
                confirmButtonText: 'Entendido'
            });
            return;
        }

        var formData = new FormData();
        formData.append("archivo", fileInput.files[0]);

        Swal.fire({
            title: 'Actualizando precios...',
            html: 'Por favor espere mientras se procesa el archivo.',
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => { Swal.showLoading(); }
        });

        $.ajax({
            url: urlSubirCosto,
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (response) {
                Swal.close();
                $("#fileInputCosto").val("");

                if (response.HUBO_ERROR === true) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error al actualizar',
                        text: response.MENSAJE_ERROR,
                        confirmButtonColor: '#5d87ff',
                        confirmButtonText: 'Entendido'
                    });
                } else {
                    setTimeout(() => {
                        Swal.fire({
                            icon: 'success',
                            title: '¡Actualización completada!',
                            text: 'Los precios se procesaron correctamente. Revisa los detalles a continuación.',
                            confirmButtonText: 'Ver resultados',
                            confirmButtonColor: '#28a745',
                            timer: 5000,
                            timerProgressBar: true,
                            showConfirmButton: true,
                            allowOutsideClick: false
                        }).then((result) => {
                            if (result.isConfirmed || result.isDismissed) {
                                Charge_ImportxProdxCosto_Modal(response.Entity.Lista);
                                $("#lblErroresCosto").text(response.Entity.Errores);
                                $("#lblActualizadosCosto").text(response.Entity.Actualizados);
                                $("#lblSinCambiosCosto").text(response.Entity.SinCambios);
                            }
                        });
                    }, 1000);
                }

                ActualizarTabla(); // refresca la grid principal de productos, ya que cambiaron precios
            },
            error: function (xhr, status, error) {
                Swal.close();
                Swal.fire({
                    icon: 'error',
                    title: 'Error de conexión',
                    text: 'No se pudo procesar el archivo. Por favor, intente nuevamente.',
                    confirmButtonColor: '#5d87ff',
                    confirmButtonText: 'Entendido'
                });
            },
            complete: function () {
                $("#fileInputCosto").val("");
            }
        });
    });
});