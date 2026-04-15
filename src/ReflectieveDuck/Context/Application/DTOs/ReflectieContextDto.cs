namespace ReflectieveDuck.Context.Application.DTOs;

public record ReflectieContextDto(
    string? HuidigeStoplichtKleur,
    int? HuidigEnergieLevel,
    IReadOnlyList<PatternDto> Patronen,
    IReadOnlyList<InsightDto> Inzichten,
    int AantalFeedbackEntries,
    DateTime? LaatsteUpdate);
