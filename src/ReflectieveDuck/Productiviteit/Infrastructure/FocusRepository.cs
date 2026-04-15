using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Productiviteit.Domain.Entities;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

namespace ReflectieveDuck.Productiviteit.Infrastructure;

public class FocusRepository : IFocusRepository
{
    private readonly LocalDbContext _db;

    public FocusRepository(LocalDbContext db) => _db = db;

    public async Task<FocusSession?> GetActiveAsync(CancellationToken ct = default)
        => await _db.FocusSessions
            .Where(f => f.EndedAt == null)
            .OrderByDescending(f => f.StartedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<FocusSession> StartAsync(FocusSession session, CancellationToken ct = default)
    {
        // Sluit eventuele actieve sessie af
        var active = await GetActiveAsync(ct);
        if (active is not null)
        {
            active.EndedAt = DateTime.UtcNow;
        }

        _db.FocusSessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return session;
    }

    public async Task<FocusSession> EndAsync(int id, string? notities = null, CancellationToken ct = default)
    {
        var session = await _db.FocusSessions.FindAsync([id], ct)
            ?? throw new InvalidOperationException($"Focus sessie {id} niet gevonden.");

        session.EndedAt = DateTime.UtcNow;
        if (notities is not null)
            session.Notities = notities;

        await _db.SaveChangesAsync(ct);
        return session;
    }

    public async Task<IReadOnlyList<FocusSession>> GetHistoryAsync(int limit = 20, CancellationToken ct = default)
        => await _db.FocusSessions
            .OrderByDescending(f => f.StartedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<EnergyLog> LogEnergyAsync(EnergyLog log, CancellationToken ct = default)
    {
        _db.EnergyLogs.Add(log);
        await _db.SaveChangesAsync(ct);
        return log;
    }

    public async Task<IReadOnlyList<EnergyLog>> GetEnergyHistoryAsync(int limit = 20, CancellationToken ct = default)
        => await _db.EnergyLogs
            .OrderByDescending(e => e.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
}
