using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Context.Application.Queries;

public record GetPatternsQuery;

public class GetPatternsQueryHandler
    : IQueryHandler<GetPatternsQuery, IReadOnlyList<PatternDto>>
{
    private readonly IPatternAnalyzer _analyzer;

    public GetPatternsQueryHandler(IPatternAnalyzer analyzer) => _analyzer = analyzer;

    public async Task<IReadOnlyList<PatternDto>> HandleAsync(
        GetPatternsQuery query, CancellationToken ct = default)
        => await _analyzer.FindPatternsAsync(ct);
}
