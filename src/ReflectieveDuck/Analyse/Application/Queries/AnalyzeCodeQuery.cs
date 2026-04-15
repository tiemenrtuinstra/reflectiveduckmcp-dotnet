using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Analyse.Application.Queries;

public record AnalyzeCodeQuery(string Code, string? Taal = null);

public class AnalyzeCodeQueryHandler
    : IQueryHandler<AnalyzeCodeQuery, CodeAnalysisDto>
{
    private readonly ICodeAnalyzer _analyzer;

    public AnalyzeCodeQueryHandler(ICodeAnalyzer analyzer) => _analyzer = analyzer;

    public Task<CodeAnalysisDto> HandleAsync(
        AnalyzeCodeQuery query, CancellationToken ct = default)
        => Task.FromResult(_analyzer.Analyze(query.Code, query.Taal));
}
