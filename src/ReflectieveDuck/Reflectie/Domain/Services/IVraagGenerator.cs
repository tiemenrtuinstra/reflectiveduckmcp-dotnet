using ReflectieveDuck.Reflectie.Domain.Entities;

namespace ReflectieveDuck.Reflectie.Domain.Services;

public interface IVraagGenerator
{
    IReadOnlyList<ReflectieVraag> GenereerVragen(
        string? stoplichtKleur = null,
        int? energieLevel = null,
        int maxVragen = 5);
}
