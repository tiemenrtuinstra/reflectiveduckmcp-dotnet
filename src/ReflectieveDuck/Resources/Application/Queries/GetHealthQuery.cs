using ReflectieveDuck.Resources.Application.DTOs;
using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetHealthQuery;

public class GetHealthQueryHandler
    : IQueryHandler<GetHealthQuery, HealthDto>
{
    private readonly ResourceProvider _provider;

    public GetHealthQueryHandler(ResourceProvider provider) => _provider = provider;

    public async Task<HealthDto> HandleAsync(GetHealthQuery query, CancellationToken ct = default)
        => await _provider.GetHealthAsync(ct);
}
