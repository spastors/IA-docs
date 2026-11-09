import { useCallback, useEffect, useRef, useState } from 'react';
import { createRoot } from 'react-dom/client';
import ClubChat from './ClubChat.jsx';
import MemberDialog from './MemberDialog.jsx';
import { ageInYears, apiRequest, memberTypes, searchKey } from './roster.js';
import './styles.css';

function ClubHeader({ roster }) {
  return <header className="club-header">
    <div className="header-inner">
      <div className="brand"><span className="crest" aria-hidden="true">CB</span><span>Oficina del club</span></div>
      <div className="header-content">
        <div><h1>Club de Barrio</h1>
          <div className="totals" aria-label="Resumen de la plantilla">
            <span><b>{roster?.players.length ?? '—'}</b> jugadores</span>
            <span><b>{roster?.coaches.length ?? '—'}</b> técnicos</span>
          </div>
        </div>
        <div className="pitch" aria-hidden="true"><i /><i /><i /></div>
      </div>
    </div>
  </header>;
}

function Roster({ kind, roster, loading, error, onReload, onDialog }) {
  const metadata = memberTypes[kind];
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState('');
  const members = (roster?.[kind] ?? []).filter(member =>
    searchKey(member.name).includes(searchKey(search)) &&
    (!category || member[metadata.categoryField] === category)
  ).sort((first, second) => kind === 'players'
    ? first.shirtNumber - second.shirtNumber : first.name.localeCompare(second.name, 'es'));

  return <section className="panel roster-panel" aria-labelledby="roster-title">
    <header className="panel-heading">
      <h2 id="roster-title">{metadata.title}</h2>
      <button className="button primary" onClick={() => onDialog({ kind, mode: 'create' })}>
        <span aria-hidden="true">＋</span> Añadir {metadata.singular}
      </button>
    </header>
    <div className="filters">
      <label className="search"><span className="sr-only">Buscar por nombre</span>
        <span aria-hidden="true" className="search-icon"></span>
        <input type="search" value={search} placeholder="Buscar por nombre" onChange={event => setSearch(event.target.value)} />
      </label>
      <label><span className="sr-only">Filtrar por {metadata.categoryLabel.toLocaleLowerCase('es')}</span>
        <select value={category} onChange={event => setCategory(event.target.value)}>
          <option value="">{metadata.allCategories}</option>
          {Object.entries(metadata.categories).map(([key, value]) => <option key={key} value={key}>{value}</option>)}
        </select>
      </label>
      <button type="button" className="text-button reload" onClick={onReload} disabled={loading}>Actualizar</button>
    </div>
    {error && <p role="alert" className="alert">{error}</p>}
    {loading && <p role="status" className="status">Actualizando la plantilla…</p>}
    {!loading && !error && members.length === 0 && <p className="empty">{
      !roster?.[kind].length ? 'Todavía no hay fichas en esta plantilla.' : 'Ninguna ficha coincide con estos filtros.'
    }</p>}
    {members.length > 0 && <table className="member-table">
      <caption className="sr-only">{metadata.title}</caption>
      <thead><tr><th scope="col">Nombre</th><th scope="col">{metadata.categoryLabel}</th><th scope="col">Edad</th>
        <th scope="col">Nacionalidad</th><th scope="col"><span className="sr-only">Acciones</span></th></tr></thead>
      <tbody>{members.map(member => <tr key={member.id}>
        <th scope="row"><span className="identity">
          {kind === 'players' && <span className="shirt" aria-label={'Dorsal ' + member.shirtNumber}>{member.shirtNumber}</span>}
          <span>{member.name}</span></span></th>
        <td className="category-cell">{metadata.categories[member[metadata.categoryField]]}</td>
        <td className="age-cell">{ageInYears(member.birthDate)} años</td>
        <td className="country-cell">{member.nationality}</td>
        <td className="actions">
          <button aria-label={'Editar la ficha de ' + member.name} onClick={() => onDialog({ kind, member, mode: 'edit' })}>Editar</button>
          <button aria-label={'Dar de baja a ' + member.name} onClick={() => onDialog({ kind, member, mode: 'delete' })}>Dar de baja</button>
        </td>
      </tr>)}</tbody>
    </table>}
  </section>;
}

const sections = { players: 'Jugadores', coaches: 'Cuerpo técnico', chat: 'Asistente' };

function App() {
  const [section, setSection] = useState('players');
  const [roster, setRoster] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [dialog, setDialog] = useState(null);
  const latestRequest = useRef(0);

  const reload = useCallback(async () => {
    // Older reads must not overwrite a refresh requested after a mutation.
    const request = ++latestRequest.current;
    setLoading(true);
    setError('');
    try {
      const [players, coaches] = await Promise.all([apiRequest('players'), apiRequest('coaches')]);
      if (request === latestRequest.current) setRoster({ players, coaches });
    } catch (failure) {
      if (request === latestRequest.current) setError('No se pudo actualizar la plantilla. ' + failure.message);
    } finally {
      if (request === latestRequest.current) setLoading(false);
    }
  }, []);

  useEffect(() => { reload(); }, [reload]);

  function selectSection(value) {
    if (value === section) return;
    setSection(value);
    setNotice('');
    if (value !== 'chat') reload();
  }

  return <>
    <a className="skip-link" href="#main">Saltar a la gestión</a>
    <ClubHeader roster={roster} />
    <div className="workspace">
      <nav className="section-nav" aria-label="Secciones del club">
        {Object.entries(sections).map(([value, label]) =>
          <button key={value} aria-pressed={section === value} onClick={() => selectSection(value)}>{label}</button>)}
      </nav>
      <main id="main" tabIndex="-1">
        {notice && <p className="notice" role="status">{notice}</p>}
        {section !== 'chat' && <Roster key={section} kind={section} roster={roster} loading={loading} error={error}
          onReload={reload} onDialog={setDialog} />}
        <ClubChat visible={section === 'chat'} onRosterChanged={reload} />
      </main>
    </div>
    {dialog && <MemberDialog {...dialog} onClose={() => setDialog(null)} onSaved={message => {
      setDialog(null);
      setNotice(message);
      reload();
    }} />}
  </>;
}

createRoot(document.getElementById('root')).render(<App />);
