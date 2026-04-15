using ReflectieveDuck.Stoplichtplan.Domain.Entities;

namespace ReflectieveDuck.Stoplichtplan.Domain.Repositories;

public interface IStoplichtRepository
{
    Task<StoplichtStatus?> GetCurrentAsync(CancellationToken ct = default);
    Task<StoplichtStatus?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<StoplichtStatus>> GetHistoryAsync(int limit = 10, CancellationToken ct = default);
    Task<StoplichtStatus> AddAsync(StoplichtStatus status, CancellationToken ct = default);
}
