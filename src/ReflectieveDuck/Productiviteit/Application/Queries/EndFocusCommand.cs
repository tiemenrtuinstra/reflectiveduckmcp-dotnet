using ReflectieveDuck.Productiviteit.Application.DTOs;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Productiviteit.Application.Queries;

public record EndFocusCommand(int? Id = null, string? Notities = null);

public class EndFocusCommandHandler
    : IQueryHandler<EndFocusCommand, FocusSessionDto?>
{
    private readonly IFocusRepository _repo;

    public EndFocusCommandHandler(IFocusRepository repo) => _repo = repo;

    public async Task<FocusSessionDto?> HandleAsync(
        EndFocusCommand command, CancellationToken ct = default)
    {
        if (command.Id.HasValue)
        {
            var session = await _repo.EndAsync(command.Id.Value, command.Notities, ct);
            return StartFocusCommandHandler.ToDto(session);
        }

        var active = await _repo.GetActiveAsync(ct);
        if (active is null) return null;

        var ended = await _repo.EndAsync(active.Id, command.Notities, ct);
        return StartFocusCommandHandler.ToDto(ended);
    }
}
