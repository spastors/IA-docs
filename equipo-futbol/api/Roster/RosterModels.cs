namespace ClubDeBarrio.Api.Roster;

public sealed class Player
{
    public int Id { get; internal set; }
    public string Name { get; internal set; } = "";
    public DateOnly BirthDate { get; internal set; }
    public string Nationality { get; internal set; } = "";
    public string Position { get; internal set; } = "";
    public int ShirtNumber { get; internal set; }
}

public sealed class Coach
{
    public int Id { get; internal set; }
    public string Name { get; internal set; } = "";
    public DateOnly BirthDate { get; internal set; }
    public string Nationality { get; internal set; } = "";
    public string Role { get; internal set; } = "";
}

// Nullable input lets the service distinguish an omitted field from a value such as zero.
public sealed record PlayerInput(
    string? Name, DateOnly? BirthDate, string? Nationality, string? Position, int? ShirtNumber);

public sealed record CoachInput(string? Name, DateOnly? BirthDate, string? Nationality, string? Role);

public sealed class RosterException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
