using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetReflectieAssistentQuery;

public class GetReflectieAssistentQueryHandler
    : IQueryHandler<GetReflectieAssistentQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetReflectieAssistentQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetReflectieAssistentQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetReflectieAssistent());
}
