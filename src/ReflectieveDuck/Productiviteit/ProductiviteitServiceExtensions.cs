using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Productiviteit.Application.DTOs;
using ReflectieveDuck.Productiviteit.Application.Queries;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Productiviteit.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Productiviteit;

public static class ProductiviteitServiceExtensions
{
    public static IServiceCollection AddProductiviteit(this IServiceCollection services)
    {
        services.AddScoped<IFocusRepository, FocusRepository>();

        services.AddScoped<IQueryHandler<StartFocusCommand, FocusSessionDto>,
            StartFocusCommandHandler>();

        services.AddScoped<IQueryHandler<EndFocusCommand, FocusSessionDto?>,
            EndFocusCommandHandler>();

        services.AddScoped<IQueryHandler<LogEnergyCommand, EnergyLogDto>,
            LogEnergyCommandHandler>();

        services.AddScoped<IQueryHandler<AnalyzeProductivityQuery, ProductivityReportDto>,
            AnalyzeProductivityQueryHandler>();

        return services;
    }
}
