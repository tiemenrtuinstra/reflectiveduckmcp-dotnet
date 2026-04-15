using ReflectieveDuck.Reflectie.Application.DTOs;
using ReflectieveDuck.Reflectie.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Reflectie.Application.Queries;

public record GenerateReflectieQuery(
    string? StoplichtKleur = null,
    int? EnergieLevel = null,
    int MaxVragen = 5);

public class GenerateReflectieQueryHandler
    : IQueryHandler<GenerateReflectieQuery, IReadOnlyList<ReflectieVraagDto>>
{
    private readonly IVraagGenerator _generator;

    public GenerateReflectieQueryHandler(IVraagGenerator generator) => _generator = generator;

    public Task<IReadOnlyList<ReflectieVraagDto>> HandleAsync(
        GenerateReflectieQuery query, CancellationToken ct = default)
    {
        var vragen = _generator.GenereerVragen(
            query.StoplichtKleur, query.EnergieLevel, query.MaxVragen);

        IReadOnlyList<ReflectieVraagDto> result = vragen
            .Select(v => new ReflectieVraagDto(v.Vraag, v.Categorie, v.Context))
            .ToList();

        return Task.FromResult(result);
    }
}
