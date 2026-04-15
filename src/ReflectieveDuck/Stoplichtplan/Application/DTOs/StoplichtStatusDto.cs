namespace ReflectieveDuck.Stoplichtplan.Application.DTOs;

public record StoplichtStatusDto(
    int Id,
    string Kleur,
    int EnergieLevel,
    string? Notities,
    DateTime CreatedAt);
