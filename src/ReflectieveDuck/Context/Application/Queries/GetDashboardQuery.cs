using Microsoft.EntityFrameworkCore;
using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Feedback.Domain.ValueObjects;
using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

namespace ReflectieveDuck.Context.Application.Queries;

public record GetDashboardQuery;

public class GetDashboardQueryHandler
    : IQueryHandler<GetDashboardQuery, DashboardDto>
{
    private readonly LocalDbContext _db;
    private readonly IPatternAnalyzer _patterns;

    public GetDashboardQueryHandler(LocalDbContext db, IPatternAnalyzer patterns)
    {
        _db = db;
        _patterns = patterns;
    }

    public async Task<DashboardDto> HandleAsync(
        GetDashboardQuery query, CancellationToken ct = default)
    {
        // Stoplicht
        var stoplicht = await _db.StoplichtHistory
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var kleur = stoplicht?.Kleur.ToString().ToLowerInvariant();

        var codewoord = kleur switch
        {
            "groen" => "Ik zit even op oranje",
            "oranje" => "Ik zit nu op rood",
            "rood" => "Ik herstel nu, nog niet groen",
            "blauw" => "Ik ben weer groen",
            _ => null
        };

        // Actieve focus sessie
        var focusSessie = await _db.FocusSessions
            .Where(f => f.EndedAt == null)
            .OrderByDescending(f => f.StartedAt)
            .FirstOrDefaultAsync(ct);

        var focusMinuten = focusSessie is not null
            ? (int?)(DateTime.UtcNow - focusSessie.StartedAt).TotalMinutes
            : null;

        // Energie trend (laatste 5)
        var energieLogs = await _db.EnergyLogs
            .OrderByDescending(e => e.CreatedAt)
            .Take(5)
            .ToListAsync(ct);

        var energieTrend = energieLogs
            .Select(e => new EnergiePunt(e.Level, e.StoplichtKleur, e.CreatedAt))
            .ToList();

        // Feedback vandaag
        var vandaag = DateTime.UtcNow.Date;
        var feedbackVandaag = await _db.FeedbackEntries
            .Where(f => f.CreatedAt >= vandaag)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

        var laatsteGevoel = feedbackVandaag
            .Where(f => f.Gevoel is not null)
            .Select(f => f.Gevoel)
            .FirstOrDefault();

        var recenteTags = feedbackVandaag
            .Where(f => f.Tags is not null)
            .SelectMany(f => FeedbackTag.Parse(f.Tags))
            .Select(t => t.Value)
            .Distinct()
            .Take(5)
            .ToList();

        // Patronen
        var patronen = await _patterns.FindPatternsAsync(ct);
        var actievePatronen = patronen
            .Take(3)
            .Select(p => p.Beschrijving)
            .ToList();

        // Advies genereren
        var advies = GenereerAdvies(kleur, focusMinuten, feedbackVandaag.Count, energieTrend);

        return new DashboardDto(
            kleur,
            stoplicht?.EnergieLevel,
            stoplicht?.Notities,
            codewoord,
            focusSessie?.Taak,
            focusSessie?.State.ToString(),
            focusMinuten,
            energieTrend,
            feedbackVandaag.Count,
            laatsteGevoel,
            recenteTags,
            actievePatronen,
            advies);
    }

    private static string GenereerAdvies(string? kleur, int? focusMinuten, int feedbackCount, List<EnergiePunt> trend)
    {
        var tips = new List<string>();

        // Kleur-gebaseerd advies
        switch (kleur)
        {
            case "rood":
                return "Je staat op rood. Stop met werken, zet communicatie uit, en zoek een stille plek. Herstel heeft nu prioriteit.";
            case "blauw":
                return "Je bent in herstel. Begin met kleine, overzichtelijke taken. Neem pauzes serieus.";
            case "oranje":
                tips.Add("Je staat op oranje — beperk prikkels, werk aan één taak tegelijk.");
                break;
        }

        // Focus sessie advies
        if (focusMinuten > 90)
            tips.Add($"Je focus sessie loopt al {focusMinuten} minuten. Neem een pauze (90-minuten regel).");

        // Feedback advies
        if (feedbackCount == 0)
            tips.Add("Je hebt vandaag nog geen feedback gelogd. Een korte check-in helpt patronen herkennen.");

        // Energie trend advies
        if (trend.Count >= 3)
        {
            var dalend = trend.Take(3).Zip(trend.Skip(1).Take(2), (a, b) => a.Level <= b.Level).All(x => x);
            if (dalend)
                tips.Add("Je energie daalt — overweeg een pauze of activiteitswisseling.");
        }

        return tips.Count > 0
            ? string.Join(" ", tips)
            : "Alles ziet er goed uit. Blijf je energie monitoren.";
    }
}
