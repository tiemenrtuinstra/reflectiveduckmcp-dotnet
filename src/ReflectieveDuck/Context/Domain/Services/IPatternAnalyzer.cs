using ReflectieveDuck.Context.Application.DTOs;

namespace ReflectieveDuck.Context.Domain.Services;

public interface IPatternAnalyzer
{
    Task<IReadOnlyList<PatternDto>> FindPatternsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<InsightDto>> GenerateInsightsAsync(CancellationToken ct = default);
    Task<ReflectieContextDto> GetFullContextAsync(CancellationToken ct = default);
}
