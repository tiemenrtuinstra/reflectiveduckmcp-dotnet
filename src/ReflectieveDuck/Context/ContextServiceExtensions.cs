using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Application.Queries;
using ReflectieveDuck.Context.Domain.Services;
using ReflectieveDuck.Context.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Context;

public static class ContextServiceExtensions
{
    public static IServiceCollection AddContext(this IServiceCollection services)
    {
        services.AddScoped<IPatternAnalyzer, PatternAnalyzer>();

        services.AddScoped<IQueryHandler<GetPatternsQuery, IReadOnlyList<PatternDto>>,
            GetPatternsQueryHandler>();

        services.AddScoped<IQueryHandler<GetInsightsQuery, IReadOnlyList<InsightDto>>,
            GetInsightsQueryHandler>();

        services.AddScoped<IQueryHandler<GetFullContextQuery, ReflectieContextDto>,
            GetFullContextQueryHandler>();

        return services;
    }
}
