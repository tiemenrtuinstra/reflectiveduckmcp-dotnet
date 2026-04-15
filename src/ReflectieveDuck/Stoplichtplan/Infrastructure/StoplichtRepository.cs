using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;
using ReflectieveDuck.Stoplichtplan.Domain.Entities;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;

namespace ReflectieveDuck.Stoplichtplan.Infrastructure;

public class StoplichtRepository : IStoplichtRepository
{
    private readonly LocalDbContext _db;

    public StoplichtRepository(LocalDbContext db) => _db = db;

    public async Task<StoplichtStatus?> GetCurrentAsync(CancellationToken ct = default)
        => await _db.StoplichtHistory
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<StoplichtStatus?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.StoplichtHistory.FindAsync([id], ct);

    public async Task<IReadOnlyList<StoplichtStatus>> GetHistoryAsync(int limit = 10, CancellationToken ct = default)
        => await _db.StoplichtHistory
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<StoplichtStatus> AddAsync(StoplichtStatus status, CancellationToken ct = default)
    {
        _db.StoplichtHistory.Add(status);
        await _db.SaveChangesAsync(ct);
        return status;
    }
}
