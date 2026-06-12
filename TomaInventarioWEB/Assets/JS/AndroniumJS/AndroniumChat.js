/*// ANDRONIUM CHAT //*/

const iconoAndronium = document.getElementById("iconoAndronium");
const contenedorChat = document.getElementById("andronium-chat");
const btnCerrarChat = document.getElementById("cerrarChat");
const inputChat = document.getElementById("andronium-input");
const btnEnviarMensaje = document.getElementById("btnEnviarMensaje");
const formChat = document.getElementById("androniumForm");


let sessionId = "";

iconoAndronium.addEventListener("click", () => {
    contenedorChat.classList.toggle("oculto-chat");
    inputChat.focus();
});

btnCerrarChat.addEventListener("click", (e) => {
    contenedorChat.classList.toggle("oculto-chat");
});

btnEnviarMensaje.addEventListener("click", () => {
    enviarMensaje();
});

formChat.addEventListener("submit", function (e) {
    e.preventDefault();
    enviarMensaje();
})



// Envia mensaje a neocortex
async function enviarMensaje() {
    const input = document.getElementById("andronium-input");
    const message = input.value.trim();
    if (!message) return;

    addMessage(message, "user");
    input.value = "";

    showLoader();

    try {
        const res = await fetch(urlEnviarMensaje, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ message, sessionId })
        });

        const data = await res.json();
        sessionId = data.sessionId;

        hideLoader();
        //console.log(data);
        addMessage(data.response, "ia");

    } catch (err) {
        hideLoader();
        addMessage("Ocurrio un error", "ia");
        console.error(err);
    }
}


// Obtiene los ultimos mensajes de neocortex
async function fetchChatHistory() {
    if (!sessionId) return;

    try {
        const res = await fetch(urlObtenerHistorial, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ sessionId })
        });

        const data = await res.json();

        //console.log(data);

        chatMessages = [];
        const cont = document.getElementById("andronium-mensajes");
        cont.innerHTML = "";

        data.messages.forEach(m => {
            const type = m.sender === "USER" ? "user" : "ia";
            renderMessage(m.content, type);
            chatMessages.push({ text: m.content, type });
        });

        saveChat();

    } catch (err) {
        console.error("Error cargando historial", err);
    }
}


// Agregar mensaje
function addMessage(text, type) {

    renderMessage(text, type);
    chatMessages.push({ text, type });
    saveChat();
}


// Colocar los mensajes en el chat visual
function renderMessage(text, type) {
    const cont = document.getElementById("andronium-mensajes");

    if (type === "user") {
        cont.innerHTML +=
            `<div class="text-end mb-1  ">
                        <span class="msg-user text-start text-white bg-primary rounded px-2 py-1">
                            ${text}
                        </span>
                    </div>`;
    } else {
        cont.innerHTML +=
            `<div class="text-start mb-1 px-2 py-1 bg-light rounded">
                        <span class="msg-ia text-dark">
                            ${text}
                        </span>
                    </div>`;
    }

    cont.scrollTop = cont.scrollHeight;
}



// Guardar chat
const STORAGE = sessionStorage;
const STORAGE_KEY = "andronium_chat";

function saveChat() {
    const chat = {
        sessionId,
        messages: chatMessages
    };
    STORAGE.setItem(STORAGE_KEY, JSON.stringify(chat));

    //console.log(chatMessages);
}

let chatMessages = [];

function loadChat() {
    const saved = STORAGE.getItem(STORAGE_KEY);
    if (!saved) return;

    const chat = JSON.parse(saved);

    sessionId = chat.sessionId || "";

    fetchChatHistory();
    //chatMessages = chat.messages || [];

    //const cont = document.getElementById("andronium-mensajes");
    //cont.innerHTML = "";
    //chatMessages.forEach(m => renderMessage(m.text, m.type));

    //console.log(saved);
    //console.log(chatMessages);
}


// Mostrar cargador de mensaje
let cargadorElemento = null;

function showLoader() {
    const cont = document.getElementById("andronium-mensajes");

    cargadorElemento = document.createElement("div");
    cargadorElemento.innerHTML =
        `<div class="text-start mb-1 px-2 py-1 bg-light rounded">
                    <span class="msg-ia text-dark fs-2">
                        Andronium está escribiendo
                        <div class="spinner-border spinner-border-sm text-primary" role="status"></div>
                    </span>
                </div>`;

    cont.appendChild(cargadorElemento);
    cont.scrollTop = cont.scrollHeight;
}

function hideLoader() {
    if (cargadorElemento) {
        cargadorElemento.remove();
        cargadorElemento = null;
    }
}