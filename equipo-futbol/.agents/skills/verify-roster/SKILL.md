---
name: verify-roster
description: Verify Club de Barrio on explicit request using an isolated SQLite database and real REST and MCP calls. Use when the user asks to check the application.
---

# Verify the roster

From the project folder, run the bundled C# verification script with the .NET 10 SDK:

```powershell
dotnet run --configuration Debug --file .agents/skills/verify-roster/scripts/verify.cs
```

The script builds the referenced API, starts its own loopback process, creates its own temporary SQLite database and restarts the API against that file. It reports completed checks and exits with a nonzero code on failure. The regular club database is outside this check.

For a requested end-to-end check, also follow the **Chat y actualización visual** steps in the project README using a separate disposable database. Verify the actual tool result, the subsequent roster refresh, and the effect of a rejected edit. If Ollama, the model or a browser is unavailable, mark that part pending.

Report in Spanish which checks passed, the first failure with its observed result, and which checks did not run. Keep application changes separate from verification.
