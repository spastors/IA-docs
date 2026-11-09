---
name: plan-roster-change
description: Plan a change to the Club de Barrio roster, tracing the affected data, API, MCP tools and React workflows. Use when asked for an impact analysis or implementation plan.
---

# Plan a roster change

Take the requested behavior, an example and any compatibility constraints as input. Read the project README and trace an existing field from its React form to its SQLite column. Trace the equivalent MCP operation as well.

1. Describe what a club member will be able to do. Give a valid example and an input the system should reject.
2. Identify the specific files and callers affected. Check what existing rows and older requests would mean after the change. Keep validation shared by REST and MCP.
3. Order the changes so each increment has an observable result. Include a data migration when existing rows require one.

Respond in Spanish with the affected paths, decisions, and acceptance examples. A plan request ends with the plan.
