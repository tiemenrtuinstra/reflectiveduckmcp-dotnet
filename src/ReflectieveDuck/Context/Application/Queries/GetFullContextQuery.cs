using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Context.Application.Queries;

public record GetFullContextQuery;

public class GetFullContextQueryHandler
    : IQueryHandler<GetFullContextQuery, ReflectieContextDto>
{
    private readonly IPatternAnalyzer _analyzer;

    public GetFullContextQueryHandler(IPatternAnalyzer analyzer) => _analyzer = analyzer;

    public async Task<ReflectieContextDto> HandleAsync(
        GetFullContextQuery query, CancellationToken ct = default)
        => await _analyzer.GetFullContextAsync(ct);
}
