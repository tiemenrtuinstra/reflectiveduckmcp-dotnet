using ReflectieveDuck.Productiviteit.Application.DTOs;
using ReflectieveDuck.Productiviteit.Domain.Entities;
using ReflectieveDuck.Productiviteit.Domain.Repositories;
using ReflectieveDuck.Productiviteit.Domain.ValueObjects;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Productiviteit.Application.Queries;

public record StartFocusCommand(string Taak, string State = "DeepFocus");

public class StartFocusCommandHandler
    : IQueryHandler<StartFocusCommand, FocusSessionDto>
{
    private readonly IFocusRepository _repo;

    public StartFocusCommandHandler(IFocusRepository repo) => _repo = repo;

    public async Task<FocusSessionDto> HandleAsync(
        StartFocusCommand command, CancellationToken ct = default)
    {
        if (!Enum.TryParse<FocusState>(command.State, ignoreCase: true, out var state))
            throw new ArgumentException(
                $"Ongeldige focus state '{command.State}'. Gebruik: DeepFocus, LightFocus, Meeting, Break.");
        var session = new FocusSession
        {
            Taak = command.Taak,
            State = state
        };

        var saved = await _repo.StartAsync(session, ct);
        return ToDto(saved);
    }

    internal static FocusSessionDto ToDto(FocusSession s)
    {
        var duur = s.EndedAt.HasValue
            ? (int?)(s.EndedAt.Value - s.StartedAt).TotalMinutes
            : null;

        return new FocusSessionDto(
            s.Id, s.Taak, s.State.ToString(),
            s.StartedAt, s.EndedAt, s.Notities, duur);
    }
}
