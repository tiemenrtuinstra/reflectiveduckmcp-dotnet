using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Reflectie.Application.DTOs;
using ReflectieveDuck.Reflectie.Application.Queries;
using ReflectieveDuck.Reflectie.Domain.Services;
using ReflectieveDuck.Reflectie.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Reflectie;

public static class ReflectieServiceExtensions
{
    public static IServiceCollection AddReflectie(this IServiceCollection services)
    {
        services.AddSingleton<IVraagGenerator, VraagGenerator>();

        services.AddScoped<IQueryHandler<GenerateReflectieQuery, IReadOnlyList<ReflectieVraagDto>>,
            GenerateReflectieQueryHandler>();

        return services;
    }
}
