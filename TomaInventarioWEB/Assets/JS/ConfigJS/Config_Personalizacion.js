/*// LOGICA PARA GUARDAR IMAGEN EXCEL //*/
const extensionesPermitidas = ["png", "jpg", "jpeg"];
const archivoInput = document.getElementById('archivoExcel');
function validarArchivo(inputFile) {
    inputFile.classList.remove("is-valid", "is-invalid");


    if (inputFile.files.length === 0) {
        inputFile.classList.add("is-invalid");
        return false;
    }

    const file = inputFile.files[0];
    const nombreFile = file.name.toLowerCase();
    const extensionFile = nombreFile.split(".").pop();

    if (!extensionesPermitidas.includes(extensionFile)) {
        inputFile.classList.add("is-invalid");
        return false;
    }

    inputFile.classList.add("is-valid");
    return true;
}

archivoInput.addEventListener("change", function() {
    validarArchivo(this); //this
});

document.getElementById('Save_image_excel').addEventListener('click', function (e) {
 
    if (!validarArchivo(archivoInput)) {
        return;
    }

    var archivo = archivoInput.files[0];

    var formData = new FormData();
    formData.append('imagen', archivo);

    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'GuardarImagenExcel', true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            /*console.log('Imagen guardada exitosamente');*/
            alertaExito("Imagen guardada", "La imagen fue guardada correctamente");
        } else {
            /*console.log('Error al guardar la imagen');*/
            alertaErrorBackend("No se pudo guardar","El servidor rechazó la imagen enviada. Revise la extensión de la imagen e inténtelo nuevamente.");
        }
    };
    xhr.send(formData);
});

/*// LOGICA ELIMINAR IMAGEN EXCEL //*/
function EliminarImagenExcel() {
    archivoInput.value = "";
    archivoInput.classList.remove("is-valid", "is-invalid");

    $.ajax({
        url: 'EliminarImagenExcel',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                /*console.log('Imagen eliminada exitosamente');*/
                alertaExito("Imagen eliminada", "La imagen fue eliminada correctamente");
            } else {
                /*console.error('Error al eliminar la imagen');*/
                alertaErrorBackend("No se pudo eliminar", "El servidor rechazó la eliminacion. Revise si existe el logo en el proyecto." );
            }
        },
        error: function (errorThrown) {
            /*console.error('Error en la solicitud para eliminar la imagen');*/
            alertaErrorInesperado(errorThrown);
        }
    });
}

$('#Delete_image_excel').click(function () {
    Swal.fire({
        title: "Estas seguro?",
        text: "Tus reportes ya no tendran un logo.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "No usar logo"
    }).then((result) => {
        if (result.isConfirmed) {
            EliminarImagenExcel();
        }
    });
});

/*// LOGICA CAMBIAR DE COLOR AL TEMA //*/
const opcionesTema = document.querySelectorAll(".temaColor");

opcionesTema.forEach(opcion => {
    opcion.addEventListener("click", () => {
        const nuevoColor = opcion.getAttribute("data-color");

        document.documentElement.style.setProperty("--color-primario", nuevoColor);
        /*document.documentElement.style.setProperty("--color-hoverSidebar", nuevoColor);*/

        localStorage.setItem("temaColor", nuevoColor);
        /*console.log(nuevoColor);*/
    });
});

/*// LOGICA CAMBIAR DE TEMA CLARO Y OSCURO //*/
//const temaOscuro = document.querySelector("#temaOscuro");
//const temaClaro = document.querySelector("#temaClaro");

//temaOscuro.addEventListener("click", () => {
//    /*const esOscuro = document.documentElement.getAttribute("data-tema") === "oscuro";*/

//    document.documentElement.setAttribute("data-bs-theme", "dark");
//    //localStorage.setItem("tema", "oscuro");
//});

//temaClaro.addEventListener("click", () => {
//    document.documentElement.style.setProperty("--color-primario", nuevoColor);
//    document.documentElement.style.setProperty("--color-hoverSidebar", nuevoColor);

//    /*localStorage.setItem("temaColor", nuevoColor);*/
//})


/* ALERTAS */
function alertaExito(titulo, texto) {
    Swal.fire({
        title: `${titulo}`,
        text: `${texto}`,
        icon: "success"
    });
}

function alertaErrorBackend(titulo, texto) {
    Swal.fire({
        title: `${titulo}`,
        text: `${texto}`,
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