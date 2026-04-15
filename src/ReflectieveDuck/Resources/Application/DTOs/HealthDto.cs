namespace ReflectieveDuck.Resources.Application.DTOs;

public record HealthDto(
    string Status,
    string Versie,
    bool DatabaseBereikbaar,
    int AantalFeedbackEntries,
    int AantalStoplichtMetingen,
    DateTime ServerTijd);
