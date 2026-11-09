using Microsoft.EntityFrameworkCore;

namespace ClubDeBarrio.Api.Roster;

public static class RosterSeed
{
    // EnsureCreated installs the schema and its seed together. An existing, empty squad stays empty.
    public static Task InitializeAsync(RosterDbContext db, CancellationToken ct = default) =>
        db.Database.EnsureCreatedAsync(ct);

    internal static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasData(
            Player(1, "Irene Solís", new(1998, 4, 12), "España", "goalkeeper", 1),
            Player(2, "Nerea Prado", new(2001, 9, 21), "España", "defender", 3),
            Player(3, "Inés Vidal", new(2003, 2, 4), "Portugal", "defender", 4),
            Player(4, "Malena Duarte", new(1999, 7, 18), "Uruguay", "midfielder", 6),
            Player(5, "Noor Amrani", new(2002, 3, 24), "Marruecos", "midfielder", 8),
            Player(6, "Lara Montes", new(2000, 6, 30), "España", "forward", 10),
            Player(7, "Lucía Vega", new(2001, 11, 14), "España", "forward", 11),
            Player(8, "Alba Ríos", new(2004, 5, 9), "Argentina", "defender", 15));

        modelBuilder.Entity<Coach>().HasData(
            Coach(1, "Celia Pardo", new(1984, 10, 19), "España", "headCoach"),
            Coach(2, "Mateo Torres", new(1991, 1, 6), "Colombia", "assistantCoach"),
            Coach(3, "Vera Costa", new(1988, 8, 27), "Portugal", "fitnessCoach"),
            Coach(4, "Dani Valle", new(1986, 12, 10), "España", "goalkeeperCoach"));
    }

    private static Player Player(int id, string name, DateOnly birthDate, string nationality,
        string position, int shirtNumber) =>
        new() { Id = id, Name = name, BirthDate = birthDate, Nationality = nationality,
            Position = position, ShirtNumber = shirtNumber };

    private static Coach Coach(int id, string name, DateOnly birthDate, string nationality, string role) =>
        new() { Id = id, Name = name, BirthDate = birthDate, Nationality = nationality, Role = role };
}
