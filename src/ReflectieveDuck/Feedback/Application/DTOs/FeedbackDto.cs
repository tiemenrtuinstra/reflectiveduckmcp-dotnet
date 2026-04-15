namespace ReflectieveDuck.Feedback.Application.DTOs;

public record FeedbackDto(
    int Id,
    string Onderwerp,
    string? Gevoel,
    string? Inzicht,
    IReadOnlyList<string> Tags,
    int? EnergieLevel,
    string? StoplichtStatus,
    string? Categorie,
    DateTime CreatedAt);
