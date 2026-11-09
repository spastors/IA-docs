using Microsoft.EntityFrameworkCore;

namespace ClubDeBarrio.Api.Roster;

public sealed class RosterDbContext(DbContextOptions<RosterDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Coach> Coaches => Set<Coach>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var players = modelBuilder.Entity<Player>();
        // The unique index also protects against two simultaneous requests for the same number.
        players.HasIndex(player => player.ShirtNumber).IsUnique();
        players.ToTable("Players", table =>
            table.HasCheckConstraint("CK_Players_ShirtNumber", "ShirtNumber BETWEEN 1 AND 99"));

        RosterSeed.Configure(modelBuilder);
    }
}
