import { useEffect, useRef, useState } from 'react';
import { apiRequest } from './roster.js';

const toolLabels = {
  list_players: 'Consultar jugadores', get_player: 'Leer ficha de jugador',
  create_player: 'Alta de jugador', update_player: 'Editar jugador', delete_player: 'Baja de jugador',
  list_coaches: 'Consultar cuerpo técnico', get_coach: 'Leer ficha de entrenador',
  create_coach: 'Alta de entrenador', update_coach: 'Editar entrenador', delete_coach: 'Baja de entrenador',
};

export default function ClubChat({ visible, onRosterChanged }) {
  const [turns, setTurns] = useState([]);
  const [draft, setDraft] = useState('');
  const [pending, setPending] = useState(false);
  const input = useRef(null);
  const history = useRef(null);

  useEffect(() => {
    if (visible && history.current) history.current.scrollTop = history.current.scrollHeight;
  }, [turns, visible]);

  async function send(event) {
    event.preventDefault();
    const question = draft.trim();
    if (!question || pending) return;

    // Keep the complete visible history, but send only recent turns to the model.
    const messages = turns.filter(turn => turn.result).flatMap(turn => [
      { role: 'user', content: turn.question },
      { role: 'assistant', content: turn.result.reply },
    ]);
    messages.push({ role: 'user', content: question });
    const previousTurns = turns;
    setTurns([...previousTurns, { question }]);
    setPending(true);
    setDraft('');
    try {
      const result = await apiRequest('chat', 'POST', { messages: messages.slice(-12) });
      setTurns([...previousTurns, { question, result }]);
      if (result.incomplete || result.operations.some(operation => operation.changed)) onRosterChanged();
    } catch (failure) {
      setTurns([...previousTurns, { question, error: failure.message }]);
      // If the connection dropped after a write, refresh before showing the roster again.
      onRosterChanged();
    } finally {
      setPending(false);
    }
  }

  function suggest(text) {
    setDraft(text);
    input.current.focus();
  }

  return (
    <section className="panel chat-panel" hidden={!visible} aria-labelledby="chat-title">
      <header className="panel-heading">
        <div><h2 id="chat-title">Asistente del club</h2><p>Consulta o modifica la plantilla con Ollama.</p></div>
        {turns.length > 0 && <button type="button" className="button soft" disabled={pending}
          onClick={() => { setTurns([]); setDraft(''); input.current.focus(); }}>Nuevo chat</button>}
      </header>
      <div className="chat-history" ref={history} role="log" aria-label="Historial de la conversación">
        {turns.length === 0 && <div className="chat-examples">
          <p>Prueba con una pregunta sobre el equipo.</p>
          <button onClick={() => suggest('¿Qué dorsales están ocupados?')}>¿Qué dorsales están ocupados?</button>
          <button onClick={() => suggest('Cambia el dorsal de Lucía Vega al 7.')}>Cambia el dorsal de Lucía Vega al 7.</button>
        </div>}
        {turns.map((turn, index) => (
          <div className="turn" key={index}>
            <div className="message user-message"><strong>Tú</strong><p>{turn.question}</p></div>
            <div className="message assistant-message">
              <strong>Asistente</strong>
              {turn.result ? <>
                <p>{turn.result.reply}</p>
                {turn.result.operations.length > 0 && <ul className="operation-list" aria-label="Operaciones MCP">
                  {turn.result.operations.map((operation, operationIndex) => (
                    <li key={operationIndex}>
                      <b>{toolLabels[operation.tool] || operation.tool}</b>
                      <span className={operation.success ? 'operation-ok' : 'operation-error'}>
                        {operation.unconfirmed ? 'Sin confirmar' : operation.success ? (operation.changed ? 'Cambio guardado' : 'Completada') : 'Rechazada'}
                      </span>
                      <p>{operation.detail}</p>
                    </li>
                  ))}
                </ul>}
                {turn.result.incomplete && <small className="partial">El asistente no pudo completar este turno.</small>}
              </> : <p>{turn.error || 'Esperando respuesta…'}</p>}
            </div>
          </div>
        ))}
      </div>
      <form onSubmit={send} className="chat-form">
        <label className="sr-only" htmlFor="chat-message">Mensaje para el asistente</label>
        <textarea id="chat-message" ref={input} rows="2" maxLength="4000" value={draft}
          placeholder="Escribe un mensaje sobre la plantilla…" onChange={event => setDraft(event.target.value)}
          onKeyDown={event => {
            if (event.key === 'Enter' && !event.shiftKey && !event.nativeEvent.isComposing) {
              event.preventDefault();
              event.currentTarget.form.requestSubmit();
            }
          }} />
        <button className="button primary" disabled={pending || !draft.trim()}>Enviar</button>
      </form>
    </section>
  );
}
