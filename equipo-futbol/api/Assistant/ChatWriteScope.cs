using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ClubDeBarrio.Api.Assistant;

// Classify the request separately from tool results so a name stored in a roster record
// cannot grant the model a new write operation during discovery.
internal sealed record ChatWriteScope(string Operation, string Target, string[] Fields)
{
    private const string Instructions = """
        Clasifica el último encargo para la plantilla del club. Las preguntas, ejemplos, simulaciones
        y negaciones son read. Selecciona una herramienta de escritura solo si el usuario pide
        ejecutar esa operación sobre una persona. Un turno puede aclarar un encargo del historial.
        Para editar o dar de baja, copia en target el nombre completo usado por el usuario,
        tal como lo escribió, o "ID 12" si indicó expresamente ese identificador.
        Un dorsal nunca es un ID. Si falta la referencia, deja target vacío.
        En fields enumera únicamente los campos de la ficha que el usuario pide cambiar.
        Los campos disponibles son name, birthDate, nationality, position, shirtNumber y role.
        """;

    public static async Task<ChatWriteScope> FromMessagesAsync(
        HttpClient ollama, string model, ChatMessage[] history, CancellationToken ct)
    {
        var schema = new
        {
            type = "object",
            properties = new
            {
                operation = new { type = "string", @enum = new[] { "read", "create_player", "update_player",
                    "delete_player", "create_coach", "update_coach", "delete_coach" } },
                target = new { type = "string" },
                fields = new { type = "array", items = new { type = "string" } }
            },
            required = new[] { "operation", "target", "fields" }
        };
        var messages = new List<ChatMessage> { new("system", Instructions) };
        messages.AddRange(history);
        using var response = await ollama.PostAsJsonAsync("/api/chat", new
        {
            model, messages, format = schema, stream = false, think = false, options = new { temperature = 0 }
        }, ct);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: ct);
        var content = body?["message"]?["content"]?.GetValue<string>() ?? throw new JsonException();
        var scope = JsonSerializer.Deserialize<ChatWriteScope>(content, JsonSerializerOptions.Web)
            ?? throw new JsonException();
        var target = Normalize(scope.Target ?? "");
        var grounded = target.Length > 0 && history.Where(message => message.Role == "user")
            .Any(message => ContainsPhrase(Normalize(message.Content), target));
        return scope with { Target = grounded ? target : "", Fields = scope.Fields ?? [] };
    }

    public bool Allows(string tool) => !IsWrite(tool) || tool == Operation;

    public static bool IsWrite(string tool) => tool.StartsWith("create_", StringComparison.Ordinal) ||
        tool.StartsWith("update_", StringComparison.Ordinal) || tool.StartsWith("delete_", StringComparison.Ordinal);

    public string? CheckIdentity(JsonElement roster, int proposedId)
    {
        if (Target.Length == 0)
            return "Indica el nombre de la persona o su ID junto con el cambio que quieres hacer.";
        var members = roster.EnumerateArray().Select(row => (
            Id: row.GetProperty("id").GetInt32(), Name: row.GetProperty("name").GetString()!)).ToArray();
        var idMatch = Regex.Match(Target, @"^id (\d+)$");
        var matches = idMatch.Success && int.TryParse(idMatch.Groups[1].Value, out var explicitId)
            ? members.Where(person => person.Id == explicitId).ToArray()
            : members.Where(person => ContainsPhrase(Normalize(person.Name), Target)).ToArray();
        if (matches.Length == 0) return "No se ha encontrado esa persona. Revisa el nombre o indica su ID.";
        if (matches.Length > 1)
        {
            var choices = string.Join(", ", matches.Select(person => $"{person.Name} (ID {person.Id})"));
            return $"Hay varias coincidencias: {choices}. ¿A cuál te refieres? Indica el ID.";
        }
        return matches[0].Id == proposedId ? null :
            $"La persona identificada es {matches[0].Name} (ID {matches[0].Id}). Repite el cambio con ese ID.";
    }

    private static bool ContainsPhrase(string text, string phrase) =>
        $" {text} ".Contains($" {phrase} ", StringComparison.Ordinal);

    private static string Normalize(string text)
    {
        var letters = text.Normalize(NormalizationForm.FormD)
            .Where(letter => CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark);
        return Regex.Replace(new string(letters.ToArray()).ToLowerInvariant(), @"[^\p{L}\p{Nd}]+", " ").Trim();
    }
}
