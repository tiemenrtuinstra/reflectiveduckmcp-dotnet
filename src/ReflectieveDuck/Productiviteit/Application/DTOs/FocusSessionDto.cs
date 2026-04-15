namespace ReflectieveDuck.Productiviteit.Application.DTOs;

public record FocusSessionDto(
    int Id,
    string Taak,
    string State,
    DateTime StartedAt,
    DateTime? EndedAt,
    string? Notities,
    int? DuurMinuten);
