---
name: manage-roster
description: Use the Club de Barrio MCP tools to answer questions about current players or coaches and carry out requested roster changes.
---

# Manage the team

Use the connected club MCP server. Discover its available tools. Take the user's operation, the kind of person, an ID or identifying details, and the fields they want to change as input.

- For a question, query the roster and answer from the returned records. A name can match several people. For a change, show the matching IDs and a distinguishing field and ask which person is meant if the target is ambiguous.
- For a creation, collect the required fields in the tool schema. For an update, use the verified ID and send only the fields the user asked to change. Use the delete tool only for a requested removal of an identified person.
- Report the returned result in Spanish. If shirt number 7 is occupied, report the conflict and leave choosing another number to the user.
- If the connection fails after submitting a write, read the record before considering a retry. If the result cannot be established, say so. Do not substitute edits to the database file for unavailable tools.
