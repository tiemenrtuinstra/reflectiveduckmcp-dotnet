using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;

namespace ReflectieveDuck.Stoplichtplan.Application.Queries;

public record GetCurrentStatusQuery;

public class GetCurrentStatusQueryHandler
    : IQueryHandler<GetCurrentStatusQuery, StoplichtStatusDto?>
{
    private readonly IStoplichtRepository _repo;

    public GetCurrentStatusQueryHandler(IStoplichtRepository repo) => _repo = repo;

    public async Task<StoplichtStatusDto?> HandleAsync(
        GetCurrentStatusQuery query, CancellationToken ct = default)
    {
        var status = await _repo.GetCurrentAsync(ct);
        return status is null ? null : ToDto(status);
    }

    internal static StoplichtStatusDto ToDto(Domain.Entities.StoplichtStatus s)
        => new(s.Id, s.Kleur.ToString().ToLowerInvariant(), s.EnergieLevel, s.Notities, s.CreatedAt);
}
