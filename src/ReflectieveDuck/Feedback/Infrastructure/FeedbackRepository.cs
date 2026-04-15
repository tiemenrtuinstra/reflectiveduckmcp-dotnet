using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Feedback.Domain.Entities;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

namespace ReflectieveDuck.Feedback.Infrastructure;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly LocalDbContext _db;

    public FeedbackRepository(LocalDbContext db) => _db = db;

    public async Task<FeedbackEntry> AddAsync(FeedbackEntry entry, CancellationToken ct = default)
    {
        _db.FeedbackEntries.Add(entry);
        await _db.SaveChangesAsync(ct);
        return entry;
    }

    public async Task<IReadOnlyList<FeedbackEntry>> GetAllAsync(int limit = 20, CancellationToken ct = default)
        => await _db.FeedbackEntries
            .OrderByDescending(f => f.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<FeedbackEntry>> SearchAsync(string zoekterm, CancellationToken ct = default)
    {
        var term = zoekterm.ToLowerInvariant();
        return await _db.FeedbackEntries
            .Where(f =>
                f.Onderwerp.ToLower().Contains(term) ||
                (f.Gevoel != null && f.Gevoel.ToLower().Contains(term)) ||
                (f.Inzicht != null && f.Inzicht.ToLower().Contains(term)))
            .OrderByDescending(f => f.CreatedAt)
            .Take(50)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<FeedbackEntry>> GetByTagAsync(string tag, CancellationToken ct = default)
    {
        var normalizedTag = tag.ToLowerInvariant();
        return await _db.FeedbackEntries
            .Where(f => f.Tags != null && f.Tags.ToLower().Contains(normalizedTag))
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }
}
