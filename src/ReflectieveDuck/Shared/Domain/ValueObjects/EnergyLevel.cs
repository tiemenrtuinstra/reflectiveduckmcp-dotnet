namespace ReflectieveDuck.Shared.Domain.ValueObjects;

public readonly struct EnergyLevel
{
    public int Value { get; }

    public EnergyLevel(int value)
    {
        Value = Math.Clamp(value, 0, 100);
    }

    public string StoplichtKleur => Value switch
    {
        >= 70 => "groen",
        >= 40 => "oranje",
        >= 15 => "rood",
        _ => "blauw"
    };

    public static implicit operator int(EnergyLevel e) => e.Value;
    public static implicit operator EnergyLevel(int v) => new(v);
}
