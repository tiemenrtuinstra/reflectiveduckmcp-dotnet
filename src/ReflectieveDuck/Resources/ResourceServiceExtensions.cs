using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Resources.Application.DTOs;
using ReflectieveDuck.Resources.Application.Queries;
using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources;

public static class ResourceServiceExtensions
{
    public static IServiceCollection AddResources(this IServiceCollection services)
    {
        services.AddScoped<ResourceProvider>();

        services.AddScoped<IQueryHandler<GetIjsbergMetafoorQuery, string>,
            GetIjsbergMetafoorQueryHandler>();

        services.AddScoped<IQueryHandler<GetAddendumQuery, string>,
            GetAddendumQueryHandler>();

        services.AddScoped<IQueryHandler<GetAssWijzerQuery, string>,
            GetAssWijzerQueryHandler>();

        services.AddScoped<IQueryHandler<GetReflectieAssistentQuery, string>,
            GetReflectieAssistentQueryHandler>();

        services.AddScoped<IQueryHandler<GetLifeMapQuery, string>,
            GetLifeMapQueryHandler>();

        services.AddScoped<IQueryHandler<GetStrengthsProfileQuery, string>,
            GetStrengthsProfileQueryHandler>();

        services.AddScoped<IQueryHandler<GetHealthQuery, HealthDto>,
            GetHealthQueryHandler>();

        services.AddScoped<IQueryHandler<GetConfigSummaryQuery, ConfigSummaryDto>,
            GetConfigSummaryQueryHandler>();

        return services;
    }
}
