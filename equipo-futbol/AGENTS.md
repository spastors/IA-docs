# Club de Barrio

This local learning project manages one football team. Keep code, identifiers and code comments in English. Use Spanish for the interface, tool results, explanations and study notes.

## Working in this project

- Read the project README for the public contracts and startup commands. The React app calls the Minimal API. The chat is a host for Ollama and a **client** of the same MCP endpoint offered to external agents.
- Put roster rules in the shared roster service. Keep the SQLite constraint protecting unique shirt numbers. Identify people by ID; names may repeat.
- Before changing a field or rule, trace the existing entity, SQLite schema, REST input/output, MCP schema, chat tool handling, React forms and lists, verification script and documentation. Update the affected callers together. Account for existing local databases when changing their schema.
- For an analysis request, describe the affected paths and acceptance examples. For an implementation request, work in small, reviewable increments.

## Project skills

- `plan-roster-change` prepares an impact analysis for a proposed change.
- `manage-roster` uses the live MCP tools when the user asks to consult or change team data. A follow-up request may refer to a person identified in an earlier turn.
- `verify-roster` provides the isolated, on-demand checks. Run them when the user asks to verify the application. Report what ran, what failed and what remains pending.

Keep generated databases, dependency folders and build output outside version control.
