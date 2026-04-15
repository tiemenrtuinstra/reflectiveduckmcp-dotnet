using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;

namespace ReflectieveDuck.Stoplichtplan.Application.Queries;

public record CompareStatusesQuery(int? VorigeId = null, int? HuidigeId = null);

public class CompareStatusesQueryHandler
    : IQueryHandler<CompareStatusesQuery, StoplichtVergelijkingDto?>
{
    private readonly IStoplichtRepository _repo;

    public CompareStatusesQueryHandler(IStoplichtRepository repo) => _repo = repo;

    public async Task<StoplichtVergelijkingDto?> HandleAsync(
        CompareStatusesQuery query, CancellationToken ct = default)
    {
        var history = await _repo.GetHistoryAsync(2, ct);
        if (history.Count < 2) return null;

        var huidige = query.HuidigeId.HasValue
            ? await _repo.GetByIdAsync(query.HuidigeId.Value, ct)
            : history[0];

        var vorige = query.VorigeId.HasValue
            ? await _repo.GetByIdAsync(query.VorigeId.Value, ct)
            : history[1];

        if (huidige is null || vorige is null) return null;

        var verschil = huidige.EnergieLevel - vorige.EnergieLevel;
        var trend = verschil switch
        {
            > 10 => "stijgend",
            < -10 => "dalend",
            _ => "stabiel"
        };

        return new StoplichtVergelijkingDto(
            GetCurrentStatusQueryHandler.ToDto(vorige),
            GetCurrentStatusQueryHandler.ToDto(huidige),
            trend,
            verschil);
    }
}
