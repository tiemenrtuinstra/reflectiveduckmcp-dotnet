using ReflectieveDuck.Productiviteit.Domain.Entities;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Shared.Domain.ValueObjects;

namespace ReflectieveDuck.Productiviteit.Application.Queries;

public record LogEnergyCommand(int Level);

public record EnergyLogDto(int Id, int Level, string StoplichtKleur, DateTime CreatedAt);

public class LogEnergyCommandHandler
    : IQueryHandler<LogEnergyCommand, EnergyLogDto>
{
    private readonly IFocusRepository _repo;

    public LogEnergyCommandHandler(IFocusRepository repo) => _repo = repo;

    public async Task<EnergyLogDto> HandleAsync(
        LogEnergyCommand command, CancellationToken ct = default)
    {
        var energie = new EnergyLevel(command.Level);
        var log = new EnergyLog
        {
            Level = energie,
            StoplichtKleur = energie.StoplichtKleur
        };

        var saved = await _repo.LogEnergyAsync(log, ct);
        return new EnergyLogDto(saved.Id, saved.Level, saved.StoplichtKleur, saved.CreatedAt);
    }
}
