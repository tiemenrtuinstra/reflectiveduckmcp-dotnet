using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Feedback.Domain.Entities;
using ReflectieveDuck.Productiviteit.Domain.Entities;
using ReflectieveDuck.Stoplichtplan.Domain.Entities;

namespace ReflectieveDuck.Shared.Infrastructure.LocalDb;

public class LocalDbContext : DbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options) { }

    public DbSet<StoplichtStatus> StoplichtHistory { get; set; } = null!;
    public DbSet<FeedbackEntry> FeedbackEntries { get; set; } = null!;
    public DbSet<FocusSession> FocusSessions { get; set; } = null!;
    public DbSet<EnergyLog> EnergyLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // OpenIddict tabellen (OAuth clients, tokens, authorizations, scopes)
        modelBuilder.UseOpenIddict();


        modelBuilder.Entity<StoplichtStatus>(e =>
        {
            e.ToTable("StoplichtHistory");
            e.Property(s => s.Kleur).HasConversion<string>();
        });

        modelBuilder.Entity<FeedbackEntry>(e =>
        {
            e.ToTable("FeedbackEntries");
        });

        modelBuilder.Entity<FocusSession>(e =>
        {
            e.ToTable("FocusSessions");
            e.Property(f => f.State).HasConversion<string>();
        });

        modelBuilder.Entity<EnergyLog>(e =>
        {
            e.ToTable("EnergyLogs");
        });
    }
}
