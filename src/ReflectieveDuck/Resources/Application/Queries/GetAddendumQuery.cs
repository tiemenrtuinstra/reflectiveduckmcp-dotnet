using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetAddendumQuery;

public class GetAddendumQueryHandler
    : IQueryHandler<GetAddendumQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetAddendumQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetAddendumQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetAddendum());
}
