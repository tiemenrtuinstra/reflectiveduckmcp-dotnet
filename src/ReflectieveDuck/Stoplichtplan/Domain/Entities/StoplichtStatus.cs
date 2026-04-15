using ReflectieveDuck.Stoplichtplan.Domain.ValueObjects;

namespace ReflectieveDuck.Stoplichtplan.Domain.Entities;

public class StoplichtStatus
{
    public int Id { get; set; }
    public StoplichtKleur Kleur { get; set; }
    public int EnergieLevel { get; set; }
    public string? Notities { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
