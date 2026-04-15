using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Application.Queries;
using ReflectieveDuck.Analyse.Domain.Services;
using ReflectieveDuck.Analyse.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Analyse;

public static class AnalyseServiceExtensions
{
    public static IServiceCollection AddAnalyse(this IServiceCollection services)
    {
        services.AddSingleton<ICodeAnalyzer, CodeAnalyzer>();
        services.AddSingleton<IStacktraceAnalyzer, StacktraceAnalyzer>();

        services.AddScoped<IQueryHandler<AnalyzeCodeQuery, CodeAnalysisDto>,
            AnalyzeCodeQueryHandler>();

        services.AddScoped<IQueryHandler<AnalyzeStacktraceQuery, StacktraceAnalysisDto>,
            AnalyzeStacktraceQueryHandler>();

        return services;
    }
}
