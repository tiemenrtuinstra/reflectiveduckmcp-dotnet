using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetIjsbergMetafoorQuery;

public class GetIjsbergMetafoorQueryHandler
    : IQueryHandler<GetIjsbergMetafoorQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetIjsbergMetafoorQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetIjsbergMetafoorQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetIjsbergMetafoor());
}
