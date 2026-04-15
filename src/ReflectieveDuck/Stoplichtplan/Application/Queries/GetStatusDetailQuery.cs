using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;

namespace ReflectieveDuck.Stoplichtplan.Application.Queries;

public record GetStatusDetailQuery(int Limit = 10);

public class GetStatusDetailQueryHandler
    : IQueryHandler<GetStatusDetailQuery, IReadOnlyList<StoplichtStatusDto>>
{
    private readonly IStoplichtRepository _repo;

    public GetStatusDetailQueryHandler(IStoplichtRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<StoplichtStatusDto>> HandleAsync(
        GetStatusDetailQuery query, CancellationToken ct = default)
    {
        var history = await _repo.GetHistoryAsync(query.Limit, ct);
        return history.Select(GetCurrentStatusQueryHandler.ToDto).ToList();
    }
}
