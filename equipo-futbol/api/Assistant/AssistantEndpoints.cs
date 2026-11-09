using ModelContextProtocol.AspNetCore;

namespace ClubDeBarrio.Api.Assistant;

public sealed record ChatMessage(string Role, string Content);
public sealed record ChatRequest(List<ChatMessage>? Messages);
public sealed record ToolOperation(string Tool, bool Success, bool Changed, string Detail, bool Unconfirmed = false);
public sealed record ChatReply(string Reply, List<ToolOperation> Operations, bool Incomplete = false);

public static class AssistantEndpoints
{
    public static IServiceCollection AddRosterAssistant(this IServiceCollection services)
    {
        services.AddMcpServer().WithHttpTransport().WithTools<RosterTools>();
        services.AddHttpClient<RosterChat>((provider, client) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(configuration["Ollama:BaseUrl"] ?? "http://127.0.0.1:11434");
            client.Timeout = TimeSpan.FromMinutes(2);
        });
        return services;
    }

    public static void MapRosterAssistant(this WebApplication app)
    {
        app.MapMcp("/mcp");
        app.MapPost("/api/chat", async (ChatRequest request, RosterChat chat, CancellationToken ct) =>
        {
            var history = request.Messages;
            if (history is not { Count: > 0 } || history[^1]?.Role != "user" ||
                history.Any(message => message is null || message.Role is not ("user" or "assistant") ||
                    string.IsNullOrWhiteSpace(message.Content) ||
                    (message.Role == "user" && message.Content.Length > 4000)))
            {
                return Results.Problem("Envía un historial de preguntas y respuestas que termine con una pregunta de hasta 4000 caracteres.",
                    statusCode: 400, title: "Conversación no válida");
            }

            var context = history.TakeLast(12).Select(message => message with
            {
                Content = message.Content[..Math.Min(4000, message.Content.Length)]
            }).ToArray();
            return Results.Ok(await chat.ReplyAsync(context, ct));
        });
    }
}
