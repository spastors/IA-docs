# Repository instructions

## Purpose

Maintain clear Spanish study notes about AI in README.md and documented, independent learning examples in their own folders.

## Documentation

- Keep explanations concise, cohesive and suitable for beginners. Explain useful technical terms instead of removing them.
- Make targeted edits and preserve existing content, examples and reviewed headings unless the task requires changing them.
- Use examples different from the user's reference material when asked for a new example.
- End each main study section with one or two meaningful comprehension questions and answers in a Markdown details block.
- Verify changing tool behavior against primary documentation and link the supporting sources near the relevant claims.

## Workflow

- Read the relevant files before editing and preserve unrelated user changes.
- Work incrementally. For substantial code changes, outline a short plan before implementation. If the user requests planning only, do not implement it.
- Before changing a shared contract, entity or business rule, trace its impact across all affected existing layers and callers: domain, application, APIs, data transfer objects, persistence, interface, configuration and documentation, as applicable. Update affected components together, including relevant tests when permitted, instead of fixing only the visible entry point.
- Keep cross-layer changes within the requested scope. Preserve compatible behavior unless the task changes it, and report any required migration, integration risk or validation that remains pending. Do not create future layers or refactor unrelated code.
- Follow the user's validation instructions. Do not run projects, tests or servers during documentation-only tasks, or when the user has prohibited them.
- Report only checks actually performed. Keep pending checks explicit.

## Important

- Write new source code, identifiers and code comments in English. Keep study notes, user-facing explanations and interface text in Spanish. Preserve existing names unless a rename is requested or necessary for the task.
- Once per conversation, after reading this file, acknowledge it with the exact phrase: "Leído, campeón."
- With that acknowledgement, ask one brief, applied comprehension question about the current topic, with two or three answer choices. Use a dedicated choice selector only if the interface supports one for quizzes. Otherwise, show the choices as text and put the answer in a collapsible Markdown details block.
- Continue the requested work without waiting for the quiz answer. If the user answers, explain briefly why their choice is correct or incorrect. Do not repeat the acknowledgement or the introductory quiz on every turn.
