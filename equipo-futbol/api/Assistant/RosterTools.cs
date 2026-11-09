using System.ComponentModel;
using System.Text.Json;
using ClubDeBarrio.Api.Roster;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace ClubDeBarrio.Api.Assistant;

[McpServerToolType]
public sealed class RosterTools
{
    private const string Positions = "Valores: goalkeeper, defender, midfielder, forward.";
    private const string Roles = "Valores: headCoach, assistantCoach, fitnessCoach, goalkeeperCoach.";

    [McpServerTool(Name = "list_players", ReadOnly = true), Description("Busca jugadores por parte del nombre o devuelve la plantilla completa. Incluye ID y dorsal.")]
    public static Task<string> ListPlayers([FromServices] RosterService roster, CancellationToken ct, string? name = null) =>
        AsJson(() => roster.ListPlayersAsync(name, ct));

    [McpServerTool(Name = "get_player", ReadOnly = true), Description("Consulta la ficha actual de un jugador por su ID.")]
    public static Task<string> GetPlayer([FromServices] RosterService roster, int id, CancellationToken ct) =>
        AsJson(() => roster.GetPlayerAsync(id, ct));

    [McpServerTool(Name = "create_player"), Description("Da de alta a un jugador con los datos indicados. El dorsal (1 a 99) debe estar libre.")]
    public static Task<string> CreatePlayer([FromServices] RosterService roster, string name, DateOnly birthDate,
        string nationality, [Description(Positions)] string position, int shirtNumber, CancellationToken ct) =>
        AsJson(() => roster.CreatePlayerAsync(new(name, birthDate, nationality, position, shirtNumber), ct));

    [McpServerTool(Name = "update_player"), Description("Actualiza al jugador identificado por ID. Incluye solo los campos que se hayan pedido cambiar.")]
    public static Task<string> UpdatePlayer([FromServices] RosterService roster, int id, CancellationToken ct,
        string? name = null, DateOnly? birthDate = null, string? nationality = null,
        [Description(Positions)] string? position = null, int? shirtNumber = null) =>
        AsJson(() => roster.PatchPlayerAsync(id, new(name, birthDate, nationality, position, shirtNumber), ct));

    [McpServerTool(Name = "delete_player", Destructive = true), Description("Da de baja al jugador con el ID indicado.")]
    public static Task<string> DeletePlayer([FromServices] RosterService roster, int id, CancellationToken ct) =>
        DeleteAsJson(id, () => roster.DeletePlayerAsync(id, ct));

    [McpServerTool(Name = "list_coaches", ReadOnly = true), Description("Busca entrenadores por parte del nombre o devuelve todo el cuerpo técnico. Incluye el ID y la función.")]
    public static Task<string> ListCoaches([FromServices] RosterService roster, CancellationToken ct, string? name = null) =>
        AsJson(() => roster.ListCoachesAsync(name, ct));

    [McpServerTool(Name = "get_coach", ReadOnly = true), Description("Consulta la ficha actual de un entrenador por su ID.")]
    public static Task<string> GetCoach([FromServices] RosterService roster, int id, CancellationToken ct) =>
        AsJson(() => roster.GetCoachAsync(id, ct));

    [McpServerTool(Name = "create_coach"), Description("Da de alta a un entrenador con los datos indicados.")]
    public static Task<string> CreateCoach([FromServices] RosterService roster, string name, DateOnly birthDate,
        string nationality, [Description(Roles)] string role, CancellationToken ct) =>
        AsJson(() => roster.CreateCoachAsync(new(name, birthDate, nationality, role), ct));

    [McpServerTool(Name = "update_coach"), Description("Actualiza al entrenador identificado por ID. Incluye solo los campos que se hayan pedido cambiar.")]
    public static Task<string> UpdateCoach([FromServices] RosterService roster, int id, CancellationToken ct,
        string? name = null, DateOnly? birthDate = null, string? nationality = null, [Description(Roles)] string? role = null) =>
        AsJson(() => roster.PatchCoachAsync(id, new(name, birthDate, nationality, role), ct));

    [McpServerTool(Name = "delete_coach", Destructive = true), Description("Da de baja al entrenador con el ID indicado.")]
    public static Task<string> DeleteCoach([FromServices] RosterService roster, int id, CancellationToken ct) =>
        DeleteAsJson(id, () => roster.DeleteCoachAsync(id, ct));

    private static Task<string> DeleteAsJson(int id, Func<Task> delete) => AsJson(async () =>
    {
        await delete();
        return new { id, deleted = true };
    });

    private static async Task<string> AsJson<T>(Func<Task<T>> action)
    {
        try
        {
            return JsonSerializer.Serialize(await action(), JsonSerializerOptions.Web);
        }
        catch (RosterException error)
        {
            // MCP exposes this message as a failed tool result, just as REST exposes a domain error.
            throw new McpException(error.Message, error);
        }
    }
}
