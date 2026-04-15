using ReflectieveDuck.Productiviteit.Domain.Entities;

namespace ReflectieveDuck.Productiviteit.Domain.Repositories;

public interface IFocusRepository
{
    Task<FocusSession?> GetActiveAsync(CancellationToken ct = default);
    Task<FocusSession> StartAsync(FocusSession session, CancellationToken ct = default);
    Task<FocusSession> EndAsync(int id, string? notities = null, CancellationToken ct = default);
    Task<IReadOnlyList<FocusSession>> GetHistoryAsync(int limit = 20, CancellationToken ct = default);
    Task<EnergyLog> LogEnergyAsync(EnergyLog log, CancellationToken ct = default);
    Task<IReadOnlyList<EnergyLog>> GetEnergyHistoryAsync(int limit = 20, CancellationToken ct = default);
}
