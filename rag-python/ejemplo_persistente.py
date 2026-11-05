"""Indexa documentos en disco y consulta el índice en otra ejecución."""

import argparse
import sys
from pathlib import Path

from rag import (
    CHROMA_DIR,
    DOCUMENTO_EJEMPLO,
    abrir_base_persistente,
    indexar_documento,
    mostrar_resultado,
    responder,
)


def main() -> None:
    argumentos = argparse.ArgumentParser(description=__doc__)
    acciones = argumentos.add_subparsers(dest="accion", required=True)

    indexar = acciones.add_parser("indexar", help="Cargar o actualizar un TXT")
    indexar.add_argument("--documento", type=Path, default=DOCUMENTO_EJEMPLO)
    consultar = acciones.add_parser("consultar", help="Preguntar sin reindexar")
    consultar.add_argument("pregunta")
    opciones = argumentos.parse_args()

    # La misma ruta y el mismo nombre de colección permiten reutilizar los datos.
    base = abrir_base_persistente()
    print(f"Base persistente: {CHROMA_DIR.resolve()}")

    if opciones.accion == "indexar":
        # Reindexar sincroniza este archivo: añade cambios y retira sus
        # fragmentos antiguos; no borra el resto de documentos de la colección.
        total, nuevos = indexar_documento(base, opciones.documento)
        print(f"Archivo indexado: {total} fragmentos; {nuevos} nuevos o modificados.")
        print("Ya puedes consultar en otra ejecución. No hace falta llamar a persist().")
    else:
        # Aquí no se carga el TXT ni se recalculan sus embeddings. Sí se calcula
        # el embedding de la nueva pregunta para buscar en el índice guardado.
        respuesta, fuentes = responder(base, opciones.pregunta)
        mostrar_resultado(respuesta, fuentes)


if __name__ == "__main__":
    try:
        main()
    except Exception as error:
        print(f"Error: {error}", file=sys.stderr)
        print("Revisa CHROMA_DIR, la indexación y la conexión con Ollama.", file=sys.stderr)
        sys.exit(1)
