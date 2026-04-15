namespace ReflectieveDuck.Context.Application.DTOs;

public record DashboardDto(
    // Stoplicht
    string? HuidigeKleur,
    int? HuidigEnergieLevel,
    string? StoplichtNotities,
    string? TransitieCodewoord,

    // Actieve focus sessie
    string? ActieveFocusTaak,
    string? ActieveFocusState,
    int? FocusMinutenActief,

    // Energie trend (laatste 5)
    IReadOnlyList<EnergiePunt> EnergieTrend,

    // Feedback samenvatting
    int AantalFeedbackVandaag,
    string? LaatsteGevoel,
    IReadOnlyList<string> RecenteTags,

    // Patronen
    IReadOnlyList<string> ActievePatronen,

    // Advies
    string Advies);

public record EnergiePunt(int Level, string Kleur, DateTime Tijdstip);
