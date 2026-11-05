"use strict";

document.addEventListener("DOMContentLoaded", () => {
  const connectionForm = document.querySelector("#connection-form");
  const serverInput = document.querySelector("#server-url");
  const modelSelect = document.querySelector("#model");
  const connectButton = document.querySelector("#connect");
  const connectLabel = document.querySelector("#connect-label");
  const connectionStatus = document.querySelector("#connection-status");
  const statusLabel = document.querySelector("#status-label");
  const activeModel = document.querySelector("#active-model");
  const chatForm = document.querySelector("#chat-form");
  const promptInput = document.querySelector("#prompt");
  const messages = document.querySelector("#messages");
  const emptyState = document.querySelector("#empty-state");
  const sendButton = document.querySelector("#send");
  const stopButton = document.querySelector("#stop");
  const newChatButton = document.querySelector("#new-chat");
  const activity = document.querySelector("#activity");
  const settings = document.querySelector("#connection-settings");

  // Contexto enviado a Ollama: preguntas y respuestas completadas, en orden.
  // Los mensajes visibles se añaden al panel sin sustituir los anteriores.
  // Solo se guarda en memoria: recargar la página comienza otra conversación.
  let history = [];
  let apiBase = "";
  let connected = false;
  let connecting = false;
  let sending = false;
  let chatController = null;

  if (window.matchMedia("(max-width: 700px)").matches) settings.open = false;

  function setStatus(state, label) {
    connectionStatus.dataset.state = state;
    statusLabel.textContent = label;
  }

  function updateControls() {
    const busy = sending || connecting;
    sendButton.disabled = busy || !connected || !modelSelect.value || !promptInput.value.trim();
    connectButton.disabled = busy;
    serverInput.disabled = busy;
    modelSelect.disabled = busy || !connected;
    newChatButton.disabled = busy;
    stopButton.hidden = !sending;
    sendButton.hidden = sending;
    connectLabel.textContent = connecting ? "Conectando…" : "Conectar";
    chatForm.setAttribute("aria-busy", String(sending));
  }

  function scrollToLatest() {
    messages.scrollTop = messages.scrollHeight;
  }

  function addMessage(role, content) {
    emptyState.remove();
    const article = document.createElement("article");
    article.className = `message message--${role}`;
    const label = document.createElement("p");
    label.className = "message-label";
    label.textContent = role === "user" ? "Tú" : "Ollama";
    const body = document.createElement("div");
    body.className = "message-body";
    // El texto del modelo nunca se interpreta como HTML.
    body.textContent = content;
    article.append(label, body);
    messages.append(article);
    scrollToLatest();
    return { article, label, body };
  }

  function addPendingMessage() {
    const message = addMessage("assistant", "");
    message.article.classList.add("message--pending");
    const dots = document.createElement("span");
    dots.className = "thinking-dots";
    dots.setAttribute("aria-hidden", "true");
    for (let i = 0; i < 3; i += 1) dots.append(document.createElement("span"));
    message.body.append(dots, document.createTextNode("Preparando la respuesta…"));
    return message;
  }

  function normalizeServer(value) {
    const url = new URL(value.trim());
    if (!["http:", "https:"].includes(url.protocol) || url.username || url.password) {
      throw new Error("Introduce una dirección HTTP o HTTPS válida para Ollama.");
    }
    if (url.pathname !== "/" || url.search || url.hash) {
      throw new Error("Usa la dirección raíz del servidor, por ejemplo http://localhost:11434, sin /api ni otros parámetros.");
    }
    return url.origin;
  }

  async function requestJson(base, path, options = {}) {
    const response = await fetch(`${base}${path}`, options);
    const raw = await response.text();
    let data;
    try {
      data = JSON.parse(raw);
    } catch {
      throw new Error(`El servidor devolvió una respuesta que no es JSON (HTTP ${response.status}). Comprueba la dirección de Ollama.`);
    }
    if (!response.ok || data?.error) {
      const detail = typeof data?.error === "string" ? data.error : response.statusText;
      throw new Error(`Ollama devolvió un error (HTTP ${response.status}): ${detail || "solicitud rechazada"}`);
    }
    return data;
  }

  function errorMessage(error) {
    if (error instanceof TypeError) {
      return "No se pudo contactar con Ollama. Comprueba que está abierto, que la dirección es correcta y que permite el origen de esta página (CORS). Consulta el README si el navegador bloquea la conexión.";
    }
    return error instanceof Error ? error.message : "Ha ocurrido un error inesperado.";
  }

  connectionForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (connecting || sending) return;
    const controller = new AbortController();
    const timeout = window.setTimeout(() => controller.abort(), 10000);
    const previousModel = modelSelect.value;
    connecting = true;
    connected = false;
    setStatus("connecting", "Conectando");
    activity.textContent = "Buscando los modelos disponibles…";
    updateControls();

    try {
      const base = normalizeServer(serverInput.value);
      const data = await requestJson(base, "/api/tags", { signal: controller.signal });
      if (!Array.isArray(data.models)) throw new Error("La lista de modelos recibida no tiene el formato esperado.");
      const names = [...new Set(data.models.map((model) => model.name).filter((name) => typeof name === "string" && name))];
      if (!names.length) {
        throw new Error("Ollama responde, pero no hay modelos disponibles. Descarga uno en la terminal, por ejemplo: ollama pull gemma4:e2b. Después vuelve a conectar.");
      }

      modelSelect.replaceChildren(...names.map((name) => new Option(name, name)));
      modelSelect.value = names.includes(previousModel) ? previousModel
        : names.includes("gemma4:e2b") ? "gemma4:e2b" : names[0];
      apiBase = base;
      serverInput.value = base;
      connected = true;
      activeModel.textContent = modelSelect.value;
      setStatus("connected", "Conectado");
      activity.textContent = "Listo para conversar.";
      if (window.matchMedia("(max-width: 700px)").matches) settings.open = false;
      promptInput.focus();
    } catch (error) {
      setStatus("error", "Revisa la conexión");
      activeModel.textContent = "Conexión pendiente";
      activity.textContent = "No se ha podido preparar la conexión.";
      alert(error.name === "AbortError" ? "Ollama no respondió en 10 segundos. Comprueba que el servidor está abierto y vuelve a conectar." : errorMessage(error));
    } finally {
      window.clearTimeout(timeout);
      connecting = false;
      updateControls();
    }
  });

  chatForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const text = promptInput.value.trim();
    if (!text || sending || connecting) return;
    if (!connected || !modelSelect.value) {
      alert("Conecta con Ollama y selecciona un modelo antes de enviar un mensaje.");
      settings.open = true;
      return;
    }

    const model = modelSelect.value;
    const userMessage = addMessage("user", text);
    const pending = addPendingMessage();
    // Cada petición incluye los turnos anteriores y la nueva pregunta.
    const currentMessages = [...history, { role: "user", content: text }];
    const startedAt = performance.now();
    chatController = new AbortController();
    stopButton.disabled = false;
    sending = true;
    promptInput.value = "";
    activity.textContent = "Ollama está generando una respuesta…";
    updateControls();

    try {
      // /api/chat recibe el historial completo que esta pestaña conserva.
      // stream: false devuelve un único objeto JSON al terminar.
      const data = await requestJson(apiBase, "/api/chat", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        signal: chatController.signal,
        body: JSON.stringify({ model, messages: currentMessages, stream: false }),
      });

      const reply = data?.message?.content;
      if (typeof reply !== "string" || !reply.trim()) {
        throw new Error("El modelo no devolvió una respuesta de texto. Prueba con otro mensaje o revisa el modelo seleccionado.");
      }

      pending.article.classList.remove("message--pending");
      pending.label.textContent = model;
      pending.body.textContent = reply.trim();
      // Solo incorporamos intercambios que han terminado correctamente.
      history = [...currentMessages, { role: "assistant", content: reply.trim() }];
      const seconds = Math.max(1, Math.round((performance.now() - startedAt) / 1000));
      activity.textContent = `Respuesta lista · ${seconds} s`;
    } catch (error) {
      pending.article.remove();
      userMessage.article.classList.add("message--failed");
      userMessage.label.textContent = "Sin respuesta · fuera del historial";
      if (!promptInput.value.trim()) promptInput.value = text;
      if (error.name === "AbortError") {
        activity.textContent = "Solicitud cancelada. Puedes volver a enviarla.";
      } else {
        activity.textContent = "No se recibió una respuesta. Puedes volver a intentarlo.";
        alert(errorMessage(error));
      }
    } finally {
      sending = false;
      chatController = null;
      updateControls();
      scrollToLatest();
      promptInput.focus();
    }
  });

  stopButton.addEventListener("click", () => {
    if (!chatController) return;
    chatController.abort();
    stopButton.disabled = true;
  });

  newChatButton.addEventListener("click", () => {
    if (sending || connecting) return;
    history = [];
    messages.replaceChildren(emptyState);
    activity.textContent = connected ? "Conversación nueva. ¿Qué tienes en mente?" : "Conecta Ollama para empezar.";
    promptInput.value = "";
    updateControls();
    promptInput.focus();
  });

  promptInput.addEventListener("input", updateControls);
  promptInput.addEventListener("keydown", (event) => {
    if (event.key === "Enter" && !event.shiftKey && !event.isComposing) {
      event.preventDefault();
      if (!sending && !connecting) chatForm.requestSubmit();
    }
  });

  serverInput.addEventListener("input", () => {
    connected = false;
    activeModel.textContent = "Conexión pendiente";
    setStatus("idle", "Sin conexión");
    activity.textContent = "Vuelve a conectar para aplicar la dirección.";
    updateControls();
  });

  modelSelect.addEventListener("change", () => {
    activeModel.textContent = modelSelect.value;
    activity.textContent = "Modelo seleccionado. El historial de la conversación se conserva.";
  });

  document.querySelectorAll("[data-prompt]").forEach((button) => {
    button.addEventListener("click", () => {
      promptInput.value = button.dataset.prompt;
      updateControls();
      promptInput.focus();
    });
  });

  updateControls();
});
