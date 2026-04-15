using ReflectieveDuck.Resources.Application.DTOs;
using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetConfigSummaryQuery;

public class GetConfigSummaryQueryHandler
    : IQueryHandler<GetConfigSummaryQuery, ConfigSummaryDto>
{
    private readonly ResourceProvider _provider;

    public GetConfigSummaryQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<ConfigSummaryDto> HandleAsync(GetConfigSummaryQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetConfigSummary());
}
