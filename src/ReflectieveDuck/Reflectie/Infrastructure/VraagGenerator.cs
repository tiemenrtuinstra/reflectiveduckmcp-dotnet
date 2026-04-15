using System.Reflection;
using ReflectieveDuck.Reflectie.Domain.Entities;
using ReflectieveDuck.Reflectie.Domain.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ReflectieveDuck.Reflectie.Infrastructure;

public class VraagGenerator : IVraagGenerator
{
    private readonly Dictionary<string, List<string>> _vragenPerKleur;

    public VraagGenerator()
    {
        _vragenPerKleur = LaadVragenUitYaml();
    }

    public IReadOnlyList<ReflectieVraag> GenereerVragen(
        string? stoplichtKleur = null,
        int? energieLevel = null,
        int maxVragen = 5)
    {
        var kleur = stoplichtKleur?.ToLowerInvariant();
        var bronVragen = kleur is not null && _vragenPerKleur.TryGetValue(kleur, out var specifiek)
            ? specifiek
            : _vragenPerKleur.GetValueOrDefault("algemeen", []);

        var categorie = kleur ?? "algemeen";

        return bronVragen
            .OrderBy(_ => Random.Shared.Next())
            .Take(maxVragen)
            .Select(v => new ReflectieVraag(v, categorie,
                energieLevel.HasValue ? $"Energielevel: {energieLevel}" : null))
            .ToList();
    }

    private static Dictionary<string, List<string>> LaadVragenUitYaml()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("reflectie_vragen.yaml", StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return FallbackVragen();

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        var yaml = reader.ReadToEnd();

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<Dictionary<string, List<string>>>(yaml)
            ?? FallbackVragen();
    }

    private static Dictionary<string, List<string>> FallbackVragen() => new()
    {
        ["algemeen"] = ["Hoe voel je je op dit moment?", "Hoe vol is je emmer (0-100)?"],
        ["groen"] = ["Wat ging er vandaag goed?", "Welke sterke punten heb je ingezet?"],
        ["oranje"] = ["Wat kost je de meeste energie?", "Welke prikkel kun je verminderen?"],
        ["rood"] = ["Wat heb je nu het meest nodig?", "Heb je al gegeten en gedronken?"],
        ["blauw"] = ["Het is oké om niets te doen.", "Heb je water binnen handbereik?"]
    };
}
