namespace ReflectieveDuck.Feedback.Domain.Entities;

public class FeedbackEntry
{
    public int Id { get; set; }
    public string Onderwerp { get; set; } = string.Empty;
    public string? Gevoel { get; set; }
    public string? Inzicht { get; set; }
    public string? Tags { get; set; }
    public int? EnergieLevel { get; set; }
    public string? StoplichtStatus { get; set; }
    public string? Categorie { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
