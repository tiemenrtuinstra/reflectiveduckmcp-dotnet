using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetLifeMapQuery;

public class GetLifeMapQueryHandler
    : IQueryHandler<GetLifeMapQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetLifeMapQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetLifeMapQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetLifeMap());
}
