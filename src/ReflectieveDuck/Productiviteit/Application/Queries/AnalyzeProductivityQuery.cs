using Microsoft.Extensions.Options;
using ReflectieveDuck.Productiviteit.Application.DTOs;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Shared.Infrastructure.Configuration;

namespace ReflectieveDuck.Productiviteit.Application.Queries;

public record AnalyzeProductivityQuery(int DagenTerug = 7);

public class AnalyzeProductivityQueryHandler
    : IQueryHandler<AnalyzeProductivityQuery, ProductivityReportDto>
{
    private readonly IFocusRepository _repo;
    private readonly ReflectieOptions _options;

    public AnalyzeProductivityQueryHandler(
        IFocusRepository repo,
        IOptions<ReflectieOptions> options)
    {
        _repo = repo;
        _options = options.Value;
    }

    public async Task<ProductivityReportDto> HandleAsync(
        AnalyzeProductivityQuery query, CancellationToken ct = default)
    {
        var sessies = await _repo.GetHistoryAsync(100, ct);
        var since = DateTime.UtcNow.AddDays(-query.DagenTerug);
        var recent = sessies.Where(s => s.StartedAt >= since).ToList();

        var afgesloten = recent.Where(s => s.EndedAt.HasValue).ToList();
        var totaalMinuten = afgesloten
            .Sum(s => (s.EndedAt!.Value - s.StartedAt).TotalMinutes);

        var gemiddeld = afgesloten.Count > 0
            ? totaalMinuten / afgesloten.Count
            : 0;

        var meestProductief = afgesloten
            .GroupBy(s => s.State.ToString())
            .OrderByDescending(g => g.Sum(s => (s.EndedAt!.Value - s.StartedAt).TotalMinutes))
            .Select(g => g.Key)
            .FirstOrDefault() ?? "Geen data";

        // Advies genereren
        string? advies = null;
        if (gemiddeld > _options.OptimalFocusMinutes)
            advies = $"Je gemiddelde sessieduur ({gemiddeld:F0} min) overschrijdt de optimale {_options.OptimalFocusMinutes} minuten. Overweeg kortere sessies met pauzes.";
        else if (afgesloten.Count == 0)
            advies = "Geen afgeronde sessies in deze periode. Probeer een focus sessie te starten!";
        else if (gemiddeld < 25)
            advies = "Je sessies zijn erg kort. Probeer langere focusblokken in te plannen voor diepere concentratie.";

        var recentDtos = recent
            .Take(10)
            .Select(StartFocusCommandHandler.ToDto)
            .ToList();

        return new ProductivityReportDto(
            afgesloten.Count,
            (int)totaalMinuten,
            gemiddeld,
            meestProductief,
            recentDtos,
            advies);
    }
}
