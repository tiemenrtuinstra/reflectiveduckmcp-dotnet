using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Context.Application.Queries;

public record GetInsightsQuery;

public class GetInsightsQueryHandler
    : IQueryHandler<GetInsightsQuery, IReadOnlyList<InsightDto>>
{
    private readonly IPatternAnalyzer _analyzer;

    public GetInsightsQueryHandler(IPatternAnalyzer analyzer) => _analyzer = analyzer;

    public async Task<IReadOnlyList<InsightDto>> HandleAsync(
        GetInsightsQuery query, CancellationToken ct = default)
        => await _analyzer.GenerateInsightsAsync(ct);
}
