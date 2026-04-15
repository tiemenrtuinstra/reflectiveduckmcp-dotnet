using ReflectieveDuck.Productiviteit.Domain.ValueObjects;

namespace ReflectieveDuck.Productiviteit.Domain.Entities;

public class FocusSession
{
    public int Id { get; set; }
    public string Taak { get; set; } = string.Empty;
    public FocusState State { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public string? Notities { get; set; }
}
