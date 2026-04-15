using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Feedback.Domain.ValueObjects;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback.Application.Queries;

public record GetFeedbackStatsQuery;

public class GetFeedbackStatsQueryHandler
    : IQueryHandler<GetFeedbackStatsQuery, FeedbackStatsDto>
{
    private readonly IFeedbackRepository _repo;

    public GetFeedbackStatsQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<FeedbackStatsDto> HandleAsync(
        GetFeedbackStatsQuery query, CancellationToken ct = default)
    {
        var all = await _repo.GetAllAsync(limit: 1000, ct);

        var topTags = all
            .Where(f => f.Tags is not null)
            .SelectMany(f => FeedbackTag.Parse(f.Tags))
            .GroupBy(t => t.Value)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TagFrequentie(g.Key, g.Count()))
            .ToList();

        var topCategorieen = all
            .Where(f => f.Categorie is not null)
            .GroupBy(f => f.Categorie!)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new CategorieFrequentie(g.Key, g.Count()))
            .ToList();

        // MED-3 fix: correct omgaan met lege set (0 is een geldige waarde)
        var energieWaarden = all
            .Where(f => f.EnergieLevel.HasValue)
            .Select(f => (double)f.EnergieLevel!.Value)
            .ToList();
        var gemiddeldEnergie = energieWaarden.Count > 0
            ? (double?)energieWaarden.Average()
            : null;

        var meestVoorkomendGevoel = all
            .Where(f => f.Gevoel is not null)
            .GroupBy(f => f.Gevoel!)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        return new FeedbackStatsDto(
            all.Count, topTags, topCategorieen,
            gemiddeldEnergie,
            meestVoorkomendGevoel);
    }
}
