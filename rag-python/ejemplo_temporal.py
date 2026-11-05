"""Carga un TXT, crea un índice en memoria y realiza una consulta RAG."""

import argparse
import sys
from pathlib import Path

from rag import (
    DOCUMENTO_EJEMPLO,
    crear_base_temporal,
    indexar_documento,
    mostrar_resultado,
    responder,
)


def main() -> None:
    argumentos = argparse.ArgumentParser(description=__doc__)
    argumentos.add_argument(
        "pregunta", nargs="?", default="¿Puedo llevarme las herramientas a casa?"
    )
    argumentos.add_argument("--documento", type=Path, default=DOCUMENTO_EJEMPLO)
    opciones = argumentos.parse_args()

    # 1. La base vive en memoria durante esta ejecución.
    base = crear_base_temporal()

    # 2. Carga, división, embeddings y almacenamiento.
    total, nuevos = indexar_documento(base, opciones.documento)
    print(f"Documento dividido en {total} fragmentos; {nuevos} incorporados.")

    # 3. Recuperación y generación. Al terminar el proceso se pierde el índice.
    respuesta, fuentes = responder(base, opciones.pregunta)
    mostrar_resultado(respuesta, fuentes)


if __name__ == "__main__":
    try:
        main()
    except Exception as error:
        # En una aplicación de consola mostramos el error y salimos con código 1.
        print(f"Error: {error}", file=sys.stderr)
        print("Revisa el archivo, Ollama y los modelos descargados.", file=sys.stderr)
        sys.exit(1)
