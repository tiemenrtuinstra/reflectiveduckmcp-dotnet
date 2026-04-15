using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetAssWijzerQuery(string? Sectie = null);

public class GetAssWijzerQueryHandler
    : IQueryHandler<GetAssWijzerQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetAssWijzerQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetAssWijzerQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetAssWijzer(query.Sectie));
}
