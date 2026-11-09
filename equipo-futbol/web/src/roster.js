export const memberTypes = {
  players: {
    title: 'Jugadores',
    singular: 'jugador',
    categoryLabel: 'Posición',
    categoryField: 'position',
    allCategories: 'Todas las posiciones',
    categories: {
      goalkeeper: 'Portería',
      defender: 'Defensa',
      midfielder: 'Centro del campo',
      forward: 'Delantera',
    },
  },
  coaches: {
    title: 'Cuerpo técnico',
    singular: 'entrenador',
    categoryLabel: 'Función',
    categoryField: 'role',
    allCategories: 'Todas las funciones',
    categories: {
      headCoach: 'Entrenador principal',
      assistantCoach: 'Ayudante',
      fitnessCoach: 'Preparación física',
      goalkeeperCoach: 'Entrenamiento de porteros',
    },
  },
};

export async function apiRequest(path, method = 'GET', body) {
  let response;
  try {
    response = await fetch('/api/' + path, {
      method,
      headers: body === undefined ? {} : { 'Content-Type': 'application/json' },
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    throw new Error('No se pudo contactar con la API. Comprueba que está en marcha.');
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    const fieldErrors = problem?.errors && Object.values(problem.errors).flat().join(' ');
    throw new Error(fieldErrors || problem?.detail || problem?.title || 'La API devolvió un error HTTP ' + response.status + '.');
  }
  if (response.status === 204) return null;
  try {
    return await response.json();
  } catch {
    throw new Error('La API ha devuelto una respuesta que no se puede leer.');
  }
}

// Compare calendar dates without interpreting a date-only string as UTC midnight.
export function ageInYears(birthDate) {
  const [year, month, day] = birthDate.split('-').map(Number);
  const today = new Date();
  const monthNow = today.getMonth() + 1;
  const birthdayAhead = monthNow < month || (monthNow === month && today.getDate() < day);
  return today.getFullYear() - year - Number(birthdayAhead);
}

export function searchKey(text) {
  return text.normalize('NFD').replace(/\p{M}/gu, '').trim().toLocaleLowerCase('es');
}
