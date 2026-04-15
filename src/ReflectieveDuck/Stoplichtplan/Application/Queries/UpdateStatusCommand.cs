using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Shared.Domain.ValueObjects;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Domain.Entities;
using ReflectieveDuck.Stoplichtplan.Domain.ValueObjects;
using ReflectieveDuck.Stoplichtplan.Domain.Repositories;

namespace ReflectieveDuck.Stoplichtplan.Application.Queries;

public record UpdateStatusCommand(string Kleur, int EnergieLevel, string? Notities = null);

public class UpdateStatusCommandHandler
    : IQueryHandler<UpdateStatusCommand, StoplichtStatusDto>
{
    private readonly IStoplichtRepository _repo;

    public UpdateStatusCommandHandler(IStoplichtRepository repo) => _repo = repo;

    public async Task<StoplichtStatusDto> HandleAsync(
        UpdateStatusCommand command, CancellationToken ct = default)
    {
        if (!Enum.TryParse<StoplichtKleur>(command.Kleur, ignoreCase: true, out var kleur))
            throw new ArgumentException(
                $"Ongeldige kleur '{command.Kleur}'. Gebruik: groen, oranje, rood, blauw.");
        var energie = new EnergyLevel(command.EnergieLevel);

        var status = new StoplichtStatus
        {
            Kleur = kleur,
            EnergieLevel = energie,
            Notities = command.Notities
        };

        var saved = await _repo.AddAsync(status, ct);
        return GetCurrentStatusQueryHandler.ToDto(saved);
    }
}
