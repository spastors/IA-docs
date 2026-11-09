import { useEffect, useRef, useState } from 'react';
import { apiRequest, memberTypes } from './roster.js';

export default function MemberDialog({ kind, mode, member, onClose, onSaved }) {
  const metadata = memberTypes[kind];
  const category = metadata.categoryField;
  const [fields, setFields] = useState({
    name: member?.name ?? '',
    nationality: member?.nationality ?? '',
    birthDate: member?.birthDate ?? '',
    [category]: member?.[category] ?? '',
    ...(kind === 'players' && { shirtNumber: member?.shirtNumber ?? '' }),
  });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const dialog = useRef(null);
  const remove = mode === 'delete';
  const actionTitle = mode === 'create' ? 'Añadir ' + metadata.singular : 'Editar ficha';

  useEffect(() => {
    const element = dialog.current;
    element.showModal();
    return () => element.close();
  }, []);

  function change(event) {
    setFields({ ...fields, [event.target.name]: event.target.value });
  }

  async function submit(event) {
    event.preventDefault();
    if (saving) return;
    setSaving(true);
    setError('');
    const path = mode === 'create' ? kind : kind + '/' + member.id;
    try {
      if (remove) {
        await apiRequest(path, 'DELETE');
      } else {
        const body = { ...fields, name: fields.name.trim(), nationality: fields.nationality.trim() };
        if (kind === 'players') body.shirtNumber = Number(fields.shirtNumber);
        await apiRequest(path, mode === 'create' ? 'POST' : 'PUT', body);
      }
    } catch (failure) {
      setError(failure.message);
      setSaving(false);
      return;
    }
    onSaved(remove ? 'Baja registrada.' : 'Ficha guardada.');
  }

  return (
    <dialog ref={dialog} aria-labelledby="dialog-title" className="member-dialog"
      onCancel={event => { event.preventDefault(); if (!saving) onClose(); }}>
      <form onSubmit={submit}>
        <header className="dialog-header">
          <h2 id="dialog-title">{remove ? 'Dar de baja' : actionTitle}</h2>
          <button className="close-button" type="button" aria-label="Cerrar" disabled={saving} onClick={onClose}>×</button>
        </header>
        {remove ? (
          <p>Se eliminará la ficha de <strong>{member.name}</strong> de la plantilla.</p>
        ) : (
          <div className="form-grid">
            <label className="wide">Nombre completo
              <input autoFocus name="name" required value={fields.name} onChange={change} />
            </label>
            <label>Fecha de nacimiento
              <input type="date" required name="birthDate" value={fields.birthDate} onChange={change} />
            </label>
            <label>Nacionalidad
              <input required name="nationality" value={fields.nationality} onChange={change} />
            </label>
            <label>{metadata.categoryLabel}
              <select required name={category} value={fields[category]} onChange={change}>
                <option value="" disabled>Seleccionar</option>
                {Object.entries(metadata.categories).map(([value, label]) => <option key={value} value={value}>{label}</option>)}
              </select>
            </label>
            {kind === 'players' && <label>Dorsal
              <input type="number" inputMode="numeric" name="shirtNumber" required min="1" max="99" step="1"
                value={fields.shirtNumber} onChange={change} />
            </label>}
          </div>
        )}
        {error && <p className="alert" role="alert">{error}</p>}
        <footer className="dialog-footer">
          <button className="button soft" type="button" disabled={saving} onClick={onClose}>Cancelar</button>
          <button className={'button ' + (remove ? 'danger' : 'primary')} disabled={saving}>
            {saving ? 'Guardando…' : remove ? 'Confirmar baja' : 'Guardar ficha'}
          </button>
        </footer>
      </form>
    </dialog>
  );
}
