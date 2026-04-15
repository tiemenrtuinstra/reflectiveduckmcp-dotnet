namespace ReflectieveDuck.Productiviteit.Domain.Entities;

public class EnergyLog
{
    public int Id { get; set; }
    public int Level { get; set; }
    public string StoplichtKleur { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
