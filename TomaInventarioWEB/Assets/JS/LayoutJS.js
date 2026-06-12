/*// LOGICA PARA SIDEBAR //*/
    // Asignar inicio a menuActivo si no hay uno
if (!localStorage.getItem("menuActivo")) {
    localStorage.setItem("menuActivo", "inicio");
}

    // Asignar inicio a menuActivo cuando se da click a la opcion Inicio en el Sidebar
document.querySelector("#Inicio")?.addEventListener("click", () => {
    localStorage.setItem("menuActivo", "inicio");
});

    // Activar la opcion guardada del sidebar al recargar la pagina
const menuGuardado = localStorage.getItem("menuActivo");
if (menuGuardado) {
    const item = document.querySelector(`.sidebar__item[data-menu="${menuGuardado}"]`);
    if (item) item.classList.add("active");
}

    // Al hacer click a alguna subopcion, guardo el id del padre
const menuItems = document.querySelectorAll(".sidebar__item");
const submenuContainer = document.querySelector(".submenu__principal");
const submenus = document.querySelectorAll(".submenu__secundario");

submenus.forEach(submenu => {
    submenu.querySelectorAll("a").forEach(link => {
        link.addEventListener("click", () => {

            const padre = submenu.id;
            localStorage.setItem("menuActivo", padre);
        });
    });
});

    // Cuando paso el mouse en alguna opcion, activa los submenus dependiendo del data-menu
menuItems.forEach(item => {
    item.addEventListener("mouseenter", () => {

        const targetMenu = item.getAttribute("data-menu");
        const targetSubMenu = document.getElementById(targetMenu);

        if (!targetSubMenu) {
            submenuContainer.classList.remove("active");
            return;
        }

        menuItems.forEach(i => i.classList.remove("active"));
        submenus.forEach(s => s.classList.remove("active"));

        item.classList.add("active");
        $('.select2-hidden-accessible').select2('close');

        targetSubMenu.classList.add("active");
        targetSubMenu.scrollTop = 0;

        submenuContainer.classList.add("active");
    });
});

    // Cuando sale el mouse del contenedor de los submenus, activo la opcion guardada inicialmente
submenuContainer.addEventListener("mouseleave", () => {

    submenuContainer.classList.remove("active");
    menuItems.forEach(i => i.classList.remove("active"));
    submenus.forEach(s => s.classList.remove("active"));

    const menuGuardado = localStorage.getItem("menuActivo");
    const item = document.querySelector(`.sidebar__item[data-menu="${menuGuardado}"]`);

    if (item) {
        item.classList.add("active");
    }
});


/*// OCULTAR Y MOSTRAR EL SIDEBAR //*/
const botonColapso = document.querySelector("#botonColapso");
const contenedorSidebar = document.querySelector(".sidebar__contenedor");
const contenidoPrincipal = document.querySelector(".contenido-principal");

botonColapso.addEventListener("click", () => {
    ocultarMostrarSideBar();
});

function ocultarMostrarSideBar() {
    const estaOculto = contenedorSidebar.classList.toggle("oculto");

    botonColapso.classList.toggle("ocultoFlecha", estaOculto);
    contenidoPrincipal.classList.toggle("extendido", estaOculto);
    submenuContainer.classList.remove("active");

    localStorage.setItem("sidebarOculto", estaOculto);
}

document.addEventListener("DOMContentLoaded", () => {
    const sidebarOculto = localStorage.getItem("sidebarOculto") === "true";

    if (sidebarOculto) {
        contenedorSidebar.classList.add("oculto");
        botonColapso.classList.add("ocultoFlecha");
        contenidoPrincipal.classList.add("extendido");
    }
});


/*// LOGICA PARA EL BUSCADOR //*/
const input = document.getElementById("buscadorSubmenus");
const resultados = document.getElementById("resultadosBusqueda");
const enlaces = Array.from(document.querySelectorAll(".submenu__secundario a")).map(a => {

    const texto = a.querySelector("span")?.textContent.trim().toLowerCase() || "";
    const padreContenedor = a.closest(".submenu__secundario");
    const padre = padreContenedor?.dataset.nombre || "";

    const busqueda = `${padre.toLowerCase()} ${texto}`;

    return {
        texto,
        padre,
        busqueda,
        href: a.getAttribute("href"),
        action: a.dataset.action || null
    }
});


input.addEventListener("input", () => {
    const query = input.value.trim().toLowerCase();
    resultados.innerHTML = "";

    if (!query) {
        resultados.classList.add("d-none");
        return;
    }

    // Guarda los resultados que va encontrando mientras se va tecleando
    const filtrados = enlaces.filter(e => e.busqueda.includes(query));

    if (filtrados.length === 0) {
        resultados.innerHTML = "<li class='list-group-item text-muted'>Sin resultados</li>";
    } else {
        filtrados.forEach(e => {
            const li = document.createElement("li");
            li.className = "list-group-item";

            // Forma la cadena que aparecera en la busqueda
            li.textContent = `${e.padre} → ${e.texto.charAt(0).toUpperCase() + e.texto.slice(1)}`;
            li.addEventListener("click", () => {
                localStorage.setItem("menuActivo", e.padre.toLowerCase());

                // Se dirige a la url
                if (e.href && e.href !== "#") {
                    window.location.href = e.href;
                    return;
                }
                // Ejecuta los modales
                if (e.action) {
                    document.querySelector(`[data-action="${e.action}"]`)?.click();
                }
            });

            resultados.appendChild(li);
        });
    }

    resultados.classList.remove("d-none");
});

    // Si se da click en cualquier lugar fuera de la lista esta se cierra
document.addEventListener("click", e => {
    if (!input.contains(e.target) && !resultados.contains(e.target)) {
        resultados.classList.add("d-none");
        /*resultadosPregunta.classList.add("d-none");*/
        input.value = "";
        //inputPregunta.value = "";
    }
});

/*// LOGICA ESTABLECER DE COLOR AL TEMA //*/
const temaColor = localStorage.getItem("temaColor");

if (temaColor) {
    document.documentElement.style.setProperty("--color-primario", temaColor);
}
 
/*// LOGICA ESTABLECER DE TEMA CLARO Y OSCURO //*/
//const tema = localStorage.getItem("tema");

//if (tema) {
//    document.documentElement.setAttribute("data-tema", tema);
//}




//$(document).ready(function () {

//    const togglePasswords = document.querySelectorAll(".togglePassword");
//    const passwords = document.querySelectorAll(".password");
//    togglePasswords.forEach(function (togglePassword, index) {
//        togglePassword.addEventListener("click", function () {

//            const type = passwords[index].getAttribute("type") === "password" ? "text" : "password";
//            passwords[index].setAttribute("type", type);

//            this.classList.toggle("fa-eye");
//        });
//    });

    
//});
//function MostrarError(msg, contenedor) {
//    if (msg.length > 0) {
//        document.getElementById(contenedor).style.visibility = "visible";
//        //$('#' + contenedor).addClass('visible');
//    } else {
//        document.getElementById(contenedor).style.visibility = "hidden";
//        //$('#' + contenedor).addClass('invisible');
//    }
//    document.getElementById(contenedor).innerHTML = `<p class="error">${msg}</p>`;
    
//}

//document.addEventListener("DOMContentLoaded", function (event) {

//    const showNavbar = (toggleId, navId, bodyId, headerId, imgId) => {
//        const toggle = document.getElementById(toggleId),
//            nav = document.getElementById(navId),
//            bodypd = document.getElementById(bodyId),
//            headerpd = document.getElementById(headerId)
//        imgpd = document.getElementById(imgId)

//        // Validate that all variables exist
//        if (toggle && nav && bodypd && headerpd) {
//            toggle.addEventListener('click', () => {
//                // show navbar
//                nav.classList.toggle('shownav')
//                // change icon
//                toggle.classList.toggle('fa-xmark')
//                // change icon
//                imgpd.classList.toggle('img_hide')
//                // add padding to body
//                bodypd.classList.toggle('body-pd')
//                // add padding to header
//                headerpd.classList.toggle('body-pd')
//            })
//        }
//    }

//    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header', 'imgTareo')

//    /*===== LINK ACTIVE =====*/
//    const linkColor = document.querySelectorAll('.nav_link')

//    function colorLink() {
//        if (linkColor) {
//            linkColor.forEach(l => l.classList.remove('active'))
//            this.classList.add('active')
//        }
//    }
//    linkColor.forEach(l => l.addEventListener('click', colorLink))

//    /*Your code to run since DOM is loaded and ready*/


//    document.querySelectorAll('.l-navbar .nav_link').forEach(function (element) {

//        element.addEventListener('click', function (e) {

//            let nextEl = element.nextElementSibling;
//            let parentEl = element.parentElement;

//            if (nextEl) {
//                e.preventDefault();
//                let mycollapse = new bootstrap.Collapse(nextEl);

//                if (nextEl.classList.contains('shownav')) {
//                    mycollapse.hide();
//                } else {
//                    mycollapse.show();
//                    // find other submenus with class=show
//                    var opened_submenu = parentEl.parentElement.querySelector('.submenu .shownav');
//                    // if it exists, then close all of them
//                    if (opened_submenu) {
//                        new bootstrap.Collapse(opened_submenu);
//                    }
//                }
//            }
//        }); // addEventListener
//    })

//     /*Obtén la referencia al elemento del enlace de cerrar sesión*/
//    const logoutLink = document.getElementById('logout-link');

//     /*Agrega un evento de clic al enlace de cerrar sesión*/
//    logoutLink.addEventListener('click', function (event) {
      

//    });

   

//});