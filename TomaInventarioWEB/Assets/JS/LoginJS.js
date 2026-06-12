/*// INGRESAR AL PRESIONAR LA TECLA ENTER //*/
document.addEventListener("DOMContentLoaded", function () {

    // revisar
    document.addEventListener('keypress', function (event) {
        if (event.key === 'Enter' && (document.querySelector("#contrasena").value && document.querySelector("#usuario").value)){
            Autenticar();
        }
    });
});


/*// OCULTAR Y MOSTRAR LA CONTRASEÑA //*/
$(document).ready(function () {

    const togglePassword = document.querySelector(".togglePassword");
    const password = document.querySelector("#contrasena");

    togglePassword.addEventListener("click", ()=> {
        const type = password.getAttribute("type") === "password" ? "text" : "password";
        password.setAttribute("type", type);

        togglePassword.classList.toggle("ti-eye");
        togglePassword.classList.toggle("ti-eye-off");
    });

});


/*// MOSTRAR MENSAJES DE ERROR QUE VIENE DEL BACKEND //*/
//function MostrarError(msg) {
//    document.querySelector("#contraseña").nextElementSibling.innerHTML = `${msg}`;
//    document.querySelector("#usuario").nextElementSibling.innerHTML="";
//    document.querySelector("#usuario").classList.add("is-invalid");
//    document.querySelector("#contraseña").classList.add("is-invalid");
//}


/*// MOSTRAR SI ES VALIDO O NO LO INGRESADO //*/
//function ValidarCampos({ user, pass }) {

//    let esValido = true;

//    const campos = [
//        { id: "usuario", valor: user },
//        { id: "contraseña", valor: pass }
//    ];

//    campos.forEach(campo => {
//        const input = document.getElementById(campo.id);
//        const mensajeOriginal = input.nextElementSibling;

//        input.classList.remove("is-invalid");

//        if (!campo.valor.trim()) {
//            mensajeOriginal.innerHTML = mensajeOriginal.dataset.msg;
//            input.classList.add("is-invalid");
//            esValido = false;
//        }
//    });

//    return esValido;
//}

function validarFormulario(form) {
    let esValido = true;
    const inputsLoginFormulario = form.querySelectorAll("input[type='text'], input[type='password']");

    inputsLoginFormulario.forEach(input => {
        input.classList.remove("is-invalid", "is-valid");

        if (input.value.trim() === "") {
            input.value = "";
            inputContrasena.nextElementSibling.textContent = inputContrasena.nextElementSibling.getAttribute("data-mensajeOriginal");
            input.classList.add("is-invalid");
            esValido = false;
        } else {
            /*input.classList.add("is-valid");*/
        }
    });

    return esValido;
}

const inputsLogin = document.querySelectorAll("#loginIngreso input[type='text'], input[type='password']")
inputsLogin.forEach(input => {
    input.addEventListener("input", () => {
        if (input.value.trim() === "") {
            /*input.classList.remove("is-valid");*/
            input.classList.add("is-invalid");
        } else {
            /*input.classList.add("is-valid");*/
            input.classList.remove("is-invalid");
        }
    });
});


/* ALERTAS */
const inputContrasena = document.querySelector("#contrasena");

function alertaLoginErrorBackend(error) {
    inputContrasena.classList.add("is-invalid");
    inputContrasena.nextElementSibling.textContent = `${error}`;
    //Swal.fire({
    //    title: "No se pudo ingresar",
    //    text: `${error}`,
    //    icon: "warning"
    //});
}

function alertaLoginErrorInesperado(error) {
    Swal.fire({
        title: "Error inesperado",
        text: "Ocurrió un problema al procesar la solicitud. Intente nuevamente.",
        icon: "error",
        footer: `<small>Detalles: ${error}</small>`
    });
}
