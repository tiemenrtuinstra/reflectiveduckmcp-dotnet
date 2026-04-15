using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Application.Queries;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;
using ReflectieveDuck.Stoplichtplan.Infrastructure;

namespace ReflectieveDuck.Stoplichtplan;

public static class StoplichtServiceExtensions
{
    public static IServiceCollection AddStoplichtplan(this IServiceCollection services)
    {
        services.AddScoped<IStoplichtRepository, StoplichtRepository>();

        services.AddScoped<IQueryHandler<GetCurrentStatusQuery, StoplichtStatusDto?>,
            GetCurrentStatusQueryHandler>();

        services.AddScoped<IQueryHandler<GetStatusDetailQuery, IReadOnlyList<StoplichtStatusDto>>,
            GetStatusDetailQueryHandler>();

        services.AddScoped<IQueryHandler<UpdateStatusCommand, StoplichtStatusDto>,
            UpdateStatusCommandHandler>();

        services.AddScoped<IQueryHandler<CompareStatusesQuery, StoplichtVergelijkingDto?>,
            CompareStatusesQueryHandler>();

        return services;
    }
}
