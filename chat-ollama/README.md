# Chat con Ollama

Un chat sencillo con HTML, CSS y JavaScript, conectado directamente a la API REST de Ollama. Incluye selector de modelos, historial de conversación, estados de carga, cancelación y errores mediante `try/catch` y `alert`.

## 1. Qué necesitas

- Ollama instalado y abierto. Puedes consultar la [guía de Ollama de estos apuntes](../README.md#14-práctica-con-ollama-ejecutar-modelos-locales).
- Un modelo descargado. Por ejemplo, desde PowerShell:

```powershell
ollama pull gemma4:e2b
```

- Python 3 para servir los archivos, o un servidor estático como Live Server de VS Code.

## 2. Abrir el proyecto

Desde la raíz de `IA-docs`, entra en la carpeta y sirve los archivos:

```powershell
cd .\chat-ollama
python -m http.server 8000 --bind 127.0.0.1
```

Mantén esa terminal abierta y visita **[http://127.0.0.1:8000](http://127.0.0.1:8000)** en el navegador. El servidor de Python entrega los archivos de la web; Ollama es quien ejecuta el modelo.

Si utilizas Live Server, abre `index.html` con esa extensión. Usa una dirección HTTP local para que la página tenga un origen definido; evita abrirla directamente como `file://`.

## 3. Usar el chat

1. En **Conexión**, deja `http://localhost:11434`, o escribe la dirección de tu servidor Ollama.
2. Pulsa **Conectar**. La aplicación consulta los modelos disponibles y llena el selector.
3. Selecciona un modelo y escribe un mensaje. **Enter** envía; **Mayús + Enter** añade otra línea.
4. Durante la solicitud, **Detener** cancela la espera desde el navegador.
5. **Nueva conversación** borra el historial de la pestaña para comenzar de nuevo.

La primera respuesta puede tardar más si el modelo tiene que cargarse en memoria. Se utiliza `stream: false`, por lo que la respuesta aparece completa al terminar. Las respuestas se muestran como texto y conservan los saltos de línea.

### Historial visible y contexto

Las preguntas y respuestas se añaden en orden al panel del chat, sin sustituir las anteriores. Puedes desplazarte hacia arriba para consultar toda la conversación de esta sesión.

Además, cada nueva petición incluye los intercambios completados. Así puedes continuar un tema sin repetir todos sus datos. Por ejemplo:

> **Tú:** Estoy preparando una charla de cinco minutos sobre reciclaje para niños de diez años.
>
> **Modelo:** Puedes organizarla en tres partes: separar residuos, reutilizar materiales y reducir envases.
>
> **Tú:** Dame una actividad para la segunda parte.

La última pregunta se envía junto con las dos intervenciones anteriores: el modelo dispone del tema, la edad del público y la referencia a «reutilizar materiales».

El historial permanece en memoria mientras la página está abierta. **Recargar la página o pulsar Nueva conversación lo borra.** Cambiar de modelo conserva los mensajes anteriores.

## 4. Cómo funciona la API REST

Una **API** permite que dos programas se comuniquen. En este ejemplo, el navegador envía peticiones HTTP a Ollama e intercambia **JSON**, un formato de datos con campos y valores.

```text
Formulario → JavaScript (fetch) → API de Ollama → modelo
                   ↑                               ↓
               conversación ← respuesta JSON ← resultado
```

| Petición | Uso en la aplicación |
| --- | --- |
| `GET /api/tags` | Obtener los modelos disponibles al conectar. |
| `POST /api/chat` | Enviar el historial y solicitar una respuesta. |

Ejemplo del cuerpo enviado a `http://localhost:11434/api/chat` al continuar la conversación anterior:

```json
{
  "model": "gemma4:e2b",
  "messages": [
    {
      "role": "user",
      "content": "Estoy preparando una charla de cinco minutos sobre reciclaje para niños de diez años."
    },
    {
      "role": "assistant",
      "content": "Puedes organizarla en tres partes: separar residuos, reutilizar materiales y reducir envases."
    },
    {
      "role": "user",
      "content": "Dame una actividad para la segunda parte."
    }
  ],
  "stream": false
}
```

`messages` mantiene el orden de la conversación: `user` identifica tus mensajes y `assistant`, las respuestas del modelo. JavaScript obtiene la nueva respuesta de **`data.message.content`**, la muestra en pantalla y la añade al historial para la siguiente petición. Esta aplicación se comunica con el servidor local y no necesita añadir una clave de API. [Referencias: API local](https://docs.ollama.com/api/introduction), [chat](https://docs.ollama.com/api/chat) y [lista de modelos](https://docs.ollama.com/api/tags).

El historial enviado consume la ventana de contexto del modelo: que un mensaje siga visible no garantiza que el modelo pueda utilizarlo si se supera su capacidad. La aplicación conserva los intercambios completados; una solicitud fallida o cancelada queda señalada en pantalla y fuera del historial que se envía después.

## 5. Si no conecta: servidor, dirección y CORS

Comprueba que Ollama está abierto, que el modelo está descargado y que la dirección corresponde a la raíz del servidor, sin añadir `/api/chat`.

**CORS** es el mecanismo con el que el servidor indica qué páginas pueden llamarlo desde un navegador. Si aparece un bloqueo de origen o un error 403, permite el origen de tu servidor estático.

Para este ejemplo, sal de Ollama desde el icono de la bandeja de Windows y abre otra terminal PowerShell:

```powershell
$env:OLLAMA_ORIGINS="http://127.0.0.1:8000,http://localhost:8000"
ollama serve
```

Mantén esa terminal abierta. El ajuste se aplica a ese proceso. Si usas otro puerto, como el de Live Server, incluye la dirección y el puerto exactos que aparecen en la barra del navegador. [Referencia: orígenes permitidos por Ollama](https://docs.ollama.com/faq#how-can-i-allow-additional-web-origins-to-access-ollama).

Los errores de conexión, las respuestas HTTP fallidas, el JSON no válido y las respuestas vacías se recogen con `try/catch` y se muestran mediante `alert`. La cancelación voluntaria se indica dentro del chat.

## 6. Archivos del proyecto

| Archivo | Responsabilidad |
| --- | --- |
| [index.html](index.html) | Estructura, formularios y controles accesibles. |
| [styles.css](styles.css) | Diseño adaptable a escritorio y móvil. |
| [app.js](app.js) | Conexión, historial, peticiones REST y manejo de errores. |

No requiere paquetes de JavaScript ni un proceso de compilación. En `app.js`, la función `requestJson` concentra las llamadas a la API. Los mensajes se incorporan al documento mediante `textContent`, para mostrarlos como texto.

**Comprueba lo aprendido**

1. El modelo responde correctamente a una pregunta, pero pierde el tema en la siguiente. Si enviases solo el mensaje nuevo, ¿qué información le estaría faltando?
2. La página se abre, pero aparece un error de CORS. ¿Por qué cambiar el texto del prompt no resolvería ese problema?

<details>
<summary>Ver respuestas</summary>

1. Le faltaría el historial que aporta el tema y los datos anteriores. Hay que incluir los mensajes pertinentes en `messages`.
2. Porque el bloqueo ocurre en la comunicación entre la página y el servidor, antes de recibir una respuesta del modelo. Hay que revisar el origen permitido y la conexión.

</details>
