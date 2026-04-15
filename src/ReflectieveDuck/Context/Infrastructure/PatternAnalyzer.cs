using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Feedback.Domain.ValueObjects;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

namespace ReflectieveDuck.Context.Infrastructure;

public class PatternAnalyzer : IPatternAnalyzer
{
    private readonly LocalDbContext _db;

    public PatternAnalyzer(LocalDbContext db) => _db = db;

    public async Task<IReadOnlyList<PatternDto>> FindPatternsAsync(CancellationToken ct = default)
    {
        var patterns = new List<PatternDto>();

        // Tag patronen
        var feedback = await _db.FeedbackEntries
            .OrderByDescending(f => f.CreatedAt)
            .Take(100)
            .ToListAsync(ct);

        var tagGroups = feedback
            .Where(f => f.Tags is not null)
            .SelectMany(f => FeedbackTag.Parse(f.Tags))
            .GroupBy(t => t.Value)
            .Where(g => g.Count() >= 3)
            .OrderByDescending(g => g.Count());

        foreach (var group in tagGroups)
        {
            patterns.Add(new PatternDto(
                "Terugkerend thema",
                $"Tag '{group.Key}' komt {group.Count()}x voor in recente feedback",
                group.Count(),
                null));
        }

        // Stoplicht patronen
        var stoplichtHistory = await _db.StoplichtHistory
            .OrderByDescending(s => s.CreatedAt)
            .Take(10)
            .ToListAsync(ct);

        if (stoplichtHistory.Count >= 3)
        {
            var recentKleuren = stoplichtHistory.Take(3).Select(s => s.Kleur.ToString().ToLowerInvariant()).ToList();
            if (recentKleuren.Distinct().Count() == 1)
            {
                patterns.Add(new PatternDto(
                    "Stabiele status",
                    $"Laatste 3 metingen zijn allemaal '{recentKleuren[0]}'",
                    3,
                    "stabiel"));
            }

            var energieTrend = stoplichtHistory.Take(5).Select(s => s.EnergieLevel).ToList();
            if (energieTrend.Count >= 3)
            {
                var isStijgend = energieTrend.Zip(energieTrend.Skip(1), (a, b) => a >= b).All(x => x);
                var isDalend = energieTrend.Zip(energieTrend.Skip(1), (a, b) => a <= b).All(x => x);
                if (isStijgend)
                    patterns.Add(new PatternDto("Energie trend", "Energie is stijgend", energieTrend.Count, "stijgend"));
                else if (isDalend)
                    patterns.Add(new PatternDto("Energie trend", "Energie is dalend", energieTrend.Count, "dalend"));
            }
        }

        return patterns;
    }

    public async Task<IReadOnlyList<InsightDto>> GenerateInsightsAsync(CancellationToken ct = default)
    {
        var insights = new List<InsightDto>();

        var feedback = await _db.FeedbackEntries
            .OrderByDescending(f => f.CreatedAt)
            .Take(50)
            .ToListAsync(ct);

        if (feedback.Count == 0)
        {
            insights.Add(new InsightDto("Tip", "Begin met het bijhouden van feedback om inzichten te genereren.", "systeem"));
            return insights;
        }

        // Gemiddeld energielevel
        var metEnergie = feedback.Where(f => f.EnergieLevel.HasValue).ToList();
        if (metEnergie.Count >= 5)
        {
            var gem = metEnergie.Average(f => f.EnergieLevel!.Value);
            insights.Add(new InsightDto(
                "Energie",
                $"Je gemiddelde energielevel is {gem:F0}/100 over de laatste {metEnergie.Count} metingen.",
                "feedback"));
        }

        // Meest voorkomend gevoel
        var gevoelens = feedback
            .Where(f => f.Gevoel is not null)
            .GroupBy(f => f.Gevoel!)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (gevoelens is not null)
        {
            insights.Add(new InsightDto(
                "Gevoel",
                $"Je meest voorkomende gevoel is '{gevoelens.Key}' ({gevoelens.Count()}x).",
                "feedback"));
        }

        return insights;
    }

    public async Task<ReflectieContextDto> GetFullContextAsync(CancellationToken ct = default)
    {
        var patronen = await FindPatternsAsync(ct);
        var inzichten = await GenerateInsightsAsync(ct);

        var huidigeStatus = await _db.StoplichtHistory
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var aantalFeedback = await _db.FeedbackEntries.CountAsync(ct);

        var laatsteUpdate = await _db.FeedbackEntries
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => (DateTime?)f.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return new ReflectieContextDto(
            huidigeStatus?.Kleur.ToString().ToLowerInvariant(),
            huidigeStatus?.EnergieLevel,
            patronen,
            inzichten,
            aantalFeedback,
            laatsteUpdate);
    }
}
