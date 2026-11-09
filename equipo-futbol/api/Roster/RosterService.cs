using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ClubDeBarrio.Api.Roster;

public sealed class RosterService(RosterDbContext db)
{
    private static readonly CompareInfo SpanishNames = CultureInfo.GetCultureInfo("es").CompareInfo;

    public async Task<List<Player>> ListPlayersAsync(string? name, CancellationToken ct)
    {
        var players = await db.Players.AsNoTracking().OrderBy(player => player.ShirtNumber).ToListAsync(ct);
        return players.Where(player => MatchesName(player.Name, name)).ToList();
    }

    public async Task<List<Coach>> ListCoachesAsync(string? name, CancellationToken ct)
    {
        var coaches = await db.Coaches.AsNoTracking().OrderBy(coach => coach.Name).ToListAsync(ct);
        return coaches.Where(coach => MatchesName(coach.Name, name)).ToList();
    }

    public async Task<Player> GetPlayerAsync(int id, CancellationToken ct) =>
        await db.Players.SingleOrDefaultAsync(player => player.Id == id, ct)
        ?? throw MissingPlayer();

    public async Task<Coach> GetCoachAsync(int id, CancellationToken ct) =>
        await db.Coaches.SingleOrDefaultAsync(coach => coach.Id == id, ct)
        ?? throw MissingCoach();

    public async Task<Player> CreatePlayerAsync(PlayerInput input, CancellationToken ct)
    {
        var player = new Player();
        Apply(player, input);
        db.Players.Add(player);
        await SaveAsync(ct);
        return player;
    }

    public async Task<Coach> CreateCoachAsync(CoachInput input, CancellationToken ct)
    {
        var coach = new Coach();
        Apply(coach, input);
        db.Coaches.Add(coach);
        await SaveAsync(ct);
        return coach;
    }

    public async Task<Player> UpdatePlayerAsync(int id, PlayerInput input, CancellationToken ct)
    {
        var player = await GetPlayerAsync(id, ct);
        Apply(player, input);
        await SaveAsync(ct);
        return player;
    }

    public async Task<Coach> UpdateCoachAsync(int id, CoachInput input, CancellationToken ct)
    {
        var coach = await GetCoachAsync(id, ct);
        Apply(coach, input);
        await SaveAsync(ct);
        return coach;
    }

    // MCP can change a single field without making the model repeat the rest of the record.
    public async Task<Player> PatchPlayerAsync(int id, PlayerInput changes, CancellationToken ct)
    {
        if (changes is { Name: null, BirthDate: null, Nationality: null, Position: null, ShirtNumber: null })
            throw NoChanges();

        var player = await GetPlayerAsync(id, ct);
        Apply(player, new PlayerInput(changes.Name ?? player.Name, changes.BirthDate ?? player.BirthDate,
            changes.Nationality ?? player.Nationality, changes.Position ?? player.Position,
            changes.ShirtNumber ?? player.ShirtNumber));
        await SaveAsync(ct);
        return player;
    }

    public async Task<Coach> PatchCoachAsync(int id, CoachInput changes, CancellationToken ct)
    {
        if (changes is { Name: null, BirthDate: null, Nationality: null, Role: null })
            throw NoChanges();

        var coach = await GetCoachAsync(id, ct);
        Apply(coach, new CoachInput(changes.Name ?? coach.Name, changes.BirthDate ?? coach.BirthDate,
            changes.Nationality ?? coach.Nationality, changes.Role ?? coach.Role));
        await SaveAsync(ct);
        return coach;
    }

    public async Task DeletePlayerAsync(int id, CancellationToken ct)
    {
        var deleted = await db.Players.Where(player => player.Id == id).ExecuteDeleteAsync(ct);
        if (deleted == 0) throw MissingPlayer();
    }

    public async Task DeleteCoachAsync(int id, CancellationToken ct)
    {
        var deleted = await db.Coaches.Where(coach => coach.Id == id).ExecuteDeleteAsync(ct);
        if (deleted == 0) throw MissingCoach();
    }

    private static void Apply(Player player, PlayerInput input)
    {
        var person = ValidatePerson(input.Name, input.BirthDate, input.Nationality);
        if (input.Position is not ("goalkeeper" or "defender" or "midfielder" or "forward"))
            throw new RosterException(400, "Elige una posición válida para el jugador.");
        if (input.ShirtNumber is null or < 1 or > 99)
            throw new RosterException(400, "El dorsal debe ser un número entero entre 1 y 99.");

        player.Name = person.Name;
        player.BirthDate = person.BirthDate;
        player.Nationality = person.Nationality;
        player.Position = input.Position;
        player.ShirtNumber = input.ShirtNumber.Value;
    }

    private static void Apply(Coach coach, CoachInput input)
    {
        var person = ValidatePerson(input.Name, input.BirthDate, input.Nationality);
        if (input.Role is not ("headCoach" or "assistantCoach" or "fitnessCoach" or "goalkeeperCoach"))
            throw new RosterException(400, "Elige una función válida para el cuerpo técnico.");

        coach.Name = person.Name;
        coach.BirthDate = person.BirthDate;
        coach.Nationality = person.Nationality;
        coach.Role = input.Role;
    }

    private static (string Name, DateOnly BirthDate, string Nationality) ValidatePerson(
        string? name, DateOnly? birthDate, string? nationality)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new RosterException(400, "Escribe un nombre.");
        if (birthDate is null || birthDate.Value >= DateOnly.FromDateTime(DateTime.Today))
            throw new RosterException(400, "La fecha de nacimiento debe ser anterior a hoy.");
        if (string.IsNullOrWhiteSpace(nationality)) throw new RosterException(400, "Indica la nacionalidad.");
        return (name.Trim(), birthDate.Value, nationality.Trim());
    }

    private async Task SaveAsync(CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        {
            // SQLITE_CONSTRAINT_UNIQUE: the roster's only unique secondary index is the shirt number.
            db.ChangeTracker.Clear();
            throw new RosterException(409, "Ese dorsal ya está asignado. Elige otro número.");
        }
        catch (DbUpdateConcurrencyException)
        {
            db.ChangeTracker.Clear();
            throw new RosterException(404, "La ficha ya no existe. Actualiza la plantilla.");
        }
    }

    private static bool MatchesName(string candidate, string? query)
    {
        // The entire local squad is small. Spanish comparison also matches “Lucia” with “Lucía”.
        return string.IsNullOrWhiteSpace(query) ||
            SpanishNames.IndexOf(candidate, query.Trim(), CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
    }

    private static RosterException MissingPlayer() => new(404, "No se ha encontrado al jugador.");
    private static RosterException MissingCoach() => new(404, "No se ha encontrado al integrante del cuerpo técnico.");
    private static RosterException NoChanges() => new(400, "Indica al menos un campo que quieras cambiar.");
}
