using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Domain.Services;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Analyse.Application.Queries;

public record AnalyzeStacktraceQuery(string Stacktrace);

public class AnalyzeStacktraceQueryHandler
    : IQueryHandler<AnalyzeStacktraceQuery, StacktraceAnalysisDto>
{
    private readonly IStacktraceAnalyzer _analyzer;

    public AnalyzeStacktraceQueryHandler(IStacktraceAnalyzer analyzer) => _analyzer = analyzer;

    public Task<StacktraceAnalysisDto> HandleAsync(
        AnalyzeStacktraceQuery query, CancellationToken ct = default)
        => Task.FromResult(_analyzer.Analyze(query.Stacktrace));
}
