using Microsoft.AspNetCore.Diagnostics;

namespace ClubDeBarrio.Api.Roster;

public static class RosterEndpoints
{
    public static void MapRosterEndpoints(this WebApplication app)
    {
        var players = app.MapGroup("/api/players");
        players.MapGet("", (string? name, RosterService roster, CancellationToken ct) =>
            roster.ListPlayersAsync(name, ct));
        players.MapGet("/{id:int}", (int id, RosterService roster, CancellationToken ct) =>
            roster.GetPlayerAsync(id, ct));
        players.MapPost("", async (PlayerInput input, RosterService roster, CancellationToken ct) =>
        {
            var player = await roster.CreatePlayerAsync(input, ct);
            return TypedResults.Created($"/api/players/{player.Id}", player);
        });
        players.MapPut("/{id:int}", (int id, PlayerInput input, RosterService roster, CancellationToken ct) =>
            roster.UpdatePlayerAsync(id, input, ct));
        players.MapDelete("/{id:int}", async (int id, RosterService roster, CancellationToken ct) =>
        {
            await roster.DeletePlayerAsync(id, ct);
            return TypedResults.NoContent();
        });

        var coaches = app.MapGroup("/api/coaches");
        coaches.MapGet("", (string? name, RosterService roster, CancellationToken ct) =>
            roster.ListCoachesAsync(name, ct));
        coaches.MapGet("/{id:int}", (int id, RosterService roster, CancellationToken ct) =>
            roster.GetCoachAsync(id, ct));
        coaches.MapPost("", async (CoachInput input, RosterService roster, CancellationToken ct) =>
        {
            var coach = await roster.CreateCoachAsync(input, ct);
            return TypedResults.Created($"/api/coaches/{coach.Id}", coach);
        });
        coaches.MapPut("/{id:int}", (int id, CoachInput input, RosterService roster, CancellationToken ct) =>
            roster.UpdateCoachAsync(id, input, ct));
        coaches.MapDelete("/{id:int}", async (int id, RosterService roster, CancellationToken ct) =>
        {
            await roster.DeleteCoachAsync(id, ct);
            return TypedResults.NoContent();
        });
    }
}

public sealed class RosterExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, detail) = exception switch
        {
            RosterException error => (error.StatusCode, error.Message),
            BadHttpRequestException error => (error.StatusCode,
                "Revisa el JSON y el tipo de cada campo. Usa el formato AAAA-MM-DD para las fechas."),
            _ => (0, "")
        };
        if (status == 0) return false;

        var title = status switch
        {
            404 => "Ficha no encontrada",
            409 => "Conflicto en la plantilla",
            _ => "Solicitud no válida"
        };
        await Results.Problem(title: title, detail: detail, statusCode: status).ExecuteAsync(context);
        return true;
    }
}
