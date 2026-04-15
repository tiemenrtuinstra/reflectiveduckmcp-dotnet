namespace ReflectieveDuck.Context.Application.DTOs;

public record PatternDto(
    string Type,
    string Beschrijving,
    int Frequentie,
    string? Trend);
