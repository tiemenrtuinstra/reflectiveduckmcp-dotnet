using ReflectieveDuck.Resources.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Resources.Application.Queries;

public record GetStrengthsProfileQuery;

public class GetStrengthsProfileQueryHandler
    : IQueryHandler<GetStrengthsProfileQuery, string>
{
    private readonly ResourceProvider _provider;

    public GetStrengthsProfileQueryHandler(ResourceProvider provider) => _provider = provider;

    public Task<string> HandleAsync(GetStrengthsProfileQuery query, CancellationToken ct = default)
        => Task.FromResult(_provider.GetStrengthsProfile());
}
