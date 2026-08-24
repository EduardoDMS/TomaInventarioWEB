/* ================================
   INGRESAR CON ENTER
================================ */

document.addEventListener("DOMContentLoaded", function () {

    document.addEventListener("keydown", function (event) {

        if (event.key !== "Enter") {
            return;
        }

        const usuario = document.querySelector("#usuario")?.value.trim();
        const contrasena = document.querySelector("#contrasena")?.value.trim();

        if (usuario && contrasena) {
            event.preventDefault();
            Autenticar(false);
        }
    });

});


/* ================================
   OCULTAR / MOSTRAR CONTRASEÑA
================================ */

$(document).ready(function () {

    const togglePassword = document.querySelector(".togglePassword");
    const password = document.querySelector("#contrasena");

    if (togglePassword && password) {

        togglePassword.addEventListener("click", () => {

            const type =
                password.getAttribute("type") === "password"
                    ? "text"
                    : "password";

            password.setAttribute("type", type);

            togglePassword.classList.toggle("ti-eye");
            togglePassword.classList.toggle("ti-eye-off");
        });
    }

});


/* ================================
   VALIDAR FORMULARIO
================================ */

function validarFormulario(form) {

    let esValido = true;

    const inputsLoginFormulario =
        form.querySelectorAll("input[type='text'], input[type='password']");

    inputsLoginFormulario.forEach(input => {

        input.classList.remove("is-invalid", "is-valid");

        if (input.value.trim() === "") {

            input.value = "";

            const mensaje = input.nextElementSibling;

            if (mensaje) {
                const mensajeOriginal =
                    mensaje.getAttribute("data-mensajeOriginal");

                if (mensajeOriginal) {
                    mensaje.textContent = mensajeOriginal;
                }
            }

            input.classList.add("is-invalid");
            esValido = false;
        }

    });

    return esValido;
}


/* ================================
   VALIDACIÓN EN TIEMPO REAL
================================ */

const inputsLogin = document.querySelectorAll(
    "#loginIngreso input[type='text'], #loginIngreso input[type='password']"
);

inputsLogin.forEach(input => {

    input.addEventListener("input", () => {

        if (input.value.trim() === "") {
            input.classList.add("is-invalid");
        } else {
            input.classList.remove("is-invalid");
        }

    });

});


/* ================================
   ALERTAS
================================ */

const inputContrasena = document.querySelector("#contrasena");

function alertaLoginErrorBackend(error) {

    inputContrasena.classList.add("is-invalid");

    inputContrasena.nextElementSibling.textContent = `${error}`;
}


function alertaLoginErrorInesperado(error) {

    Swal.fire({
        title: "Error inesperado",
        text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
        icon: "error",
        footer: `<small>Detalles: ${error}</small>`
    });

}