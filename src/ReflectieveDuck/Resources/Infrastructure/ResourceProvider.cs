using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReflectieveDuck.Resources.Application.DTOs;
using ReflectieveDuck.Shared.Infrastructure.Configuration;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

namespace ReflectieveDuck.Resources.Infrastructure;

public class ResourceProvider
{
    private readonly LocalDbContext _db;
    private readonly ReflectieOptions _options;
    private readonly CacheOptions _cacheOptions;

    public ResourceProvider(
        LocalDbContext db,
        IOptions<ReflectieOptions> options,
        IOptions<CacheOptions> cacheOptions)
    {
        _db = db;
        _options = options.Value;
        _cacheOptions = cacheOptions.Value;
    }

    public string GetIjsbergMetafoor() => ReadEmbeddedYaml("ijsbergmetafoor.yaml");
    public string GetAddendum() => ReadEmbeddedYaml("addendum.yaml");
    public string GetReflectieAssistent() => ReadEmbeddedYaml("reflectie_assistent.yaml");
    public string GetLifeMap() => ReadEmbeddedYaml("lifemap.yaml");
    public string GetStrengthsProfile() => ReadEmbeddedYaml("strengths_profile.yaml");
    public string GetStoplichtplan() => ReadEmbeddedYaml("stoplichtplan.yaml");

    /// <summary>
    /// Haal de ASS-wijzer op, optioneel gefilterd op sectie.
    /// Secties: prikkels, communicatie, veranderingen, reactie, zelfhulp, anderen.
    /// </summary>
    public string GetAssWijzer(string? sectie = null)
    {
        var full = ReadEmbeddedYaml("ass_wijzer.yaml");

        if (string.IsNullOrWhiteSpace(sectie))
            return full;

        // YAML sectie-extractie: zoek het top-level blok dat past bij de gevraagde sectie
        var sectionMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["prikkels"] = "waarLooptDeEmmerVanVol:",
            ["communicatie"] = "waarLooptDeEmmerVanVol:",
            ["veranderingen"] = "waarLooptDeEmmerVanVol:",
            ["reactie"] = "reactieBijOvervolleEmmer:",
            ["zelfhulp"] = "watHelptOmEmmerTeLegen:",
            ["anderen"] = "watKunnenAnderenDoen:"
        };

        // Sub-key mapping voor secties binnen waarLooptDeEmmerVanVol en watHelptOmEmmerTeLegen
        var subKeyMapping = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            ["prikkels"] = "prikkels:",
            ["communicatie"] = "contactCommunicatie:",
            ["veranderingen"] = "veranderingenOverzicht:",
            ["reactie"] = null, // Hele sectie
            ["zelfhulp"] = null, // Hele sectie
            ["anderen"] = null   // Hele sectie
        };

        if (!sectionMapping.ContainsKey(sectie))
            return $"Onbekende sectie '{sectie}'. Beschikbare secties: prikkels, communicatie, veranderingen, reactie, zelfhulp, anderen.";

        return ExtractYamlSection(full, sectionMapping[sectie], subKeyMapping[sectie]);
    }

    /// <summary>
    /// Haal stoplichtplan tips op voor een specifieke kleur.
    /// Retourneert: zelfondersteuning, collegaondersteuning, mindfulness, scrumAgile.
    /// </summary>
    public string GetStoplichtTips(string kleur)
    {
        var yaml = ReadEmbeddedYaml("stoplichtplan.yaml");
        var kleurLower = kleur.ToLowerInvariant();

        // Key zonder leading spaties — ExtractYamlSection vergelijkt tegen getrimde regels
        var sectie = ExtractYamlSection(yaml, $"{kleurLower}:", null);
        if (string.IsNullOrWhiteSpace(sectie) || sectie.StartsWith("Sectie"))
            return $"Kleur '{kleur}' niet gevonden. Gebruik: groen, oranje, rood, blauw.";

        return sectie;
    }

    /// <summary>
    /// Haal het transitie-codewoord op voor een kleur.
    /// </summary>
    public string GetCodewoord(string kleur)
    {
        var codewoorden = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["groen"] = "Transitie naar oranje: \"Ik zit even op oranje\"",
            ["oranje"] = "Transitie naar rood: \"Ik zit nu op rood\"",
            ["rood"] = "Transitie naar herstel: \"Ik herstel nu, nog niet groen\"",
            ["blauw"] = "Transitie naar groen: \"Ik ben weer groen\""
        };

        return codewoorden.TryGetValue(kleur, out var woord)
            ? woord
            : $"Kleur '{kleur}' niet gevonden. Gebruik: groen, oranje, rood, blauw.";
    }

    /// <summary>
    /// Haal de kernkwadranten op uit modules.yaml.
    /// </summary>
    public string GetKernkwadranten() => ExtractYamlSection(
        ReadEmbeddedYaml("modules.yaml"), "kernkwadrantAnalyse:", null);

    /// <summary>
    /// Haal een specifieke module op uit modules.yaml.
    /// </summary>
    public string GetModule(string moduleNaam) => ExtractYamlSection(
        ReadEmbeddedYaml("modules.yaml"), $"{moduleNaam}:", null);

    public async Task<HealthDto> GetHealthAsync(CancellationToken ct = default)
    {
        var dbOk = false;
        try
        {
            dbOk = await _db.Database.CanConnectAsync(ct);
        }
        catch { /* database niet bereikbaar */ }

        var feedbackCount = dbOk ? await _db.FeedbackEntries.CountAsync(ct) : 0;
        var stoplichtCount = dbOk ? await _db.StoplichtHistory.CountAsync(ct) : 0;

        return new HealthDto(
            dbOk ? "Gezond" : "Database onbereikbaar",
            "1.0.0",
            dbOk,
            feedbackCount,
            stoplichtCount,
            DateTime.UtcNow);
    }

    public ConfigSummaryDto GetConfigSummary()
    {
        return new ConfigSummaryDto(
            _options.MaxReflectieVragen,
            _options.FeedbackRetentionDays,
            _options.OptimalFocusMinutes,
            _options.MaxMeetingRatio,
            _options.MaxInterruptionsPerHour,
            _cacheOptions.FeedbackTtlMinutes);
    }

    /// <summary>
    /// Leest een embedded YAML resource uit de entry assembly.
    /// </summary>
    internal static string ReadEmbeddedYaml(string filename)
    {
        var assembly = Assembly.GetEntryAssembly()
            ?? Assembly.GetExecutingAssembly();

        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(filename, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return $"Resource '{filename}' niet gevonden.";

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Extraheert een sectie uit YAML tekst op basis van een key (en optioneel sub-key).
    /// </summary>
    private static string ExtractYamlSection(string yaml, string sectionKey, string? subKey)
    {
        var lines = yaml.Split('\n');
        var inSection = false;
        var inSubSection = subKey is null; // Als geen sub-key, pak alles in de sectie
        var sectionIndent = -1;
        var subSectionIndent = -1;
        var result = new List<string>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();
            var indent = line.Length - trimmed.Length;

            if (!inSection)
            {
                if (trimmed.StartsWith(sectionKey, StringComparison.OrdinalIgnoreCase))
                {
                    inSection = true;
                    sectionIndent = indent;
                    if (subKey is null)
                        result.Add(trimmed);
                }
                continue;
            }

            // We zitten in de sectie — check of we er nog in zitten
            if (!string.IsNullOrWhiteSpace(line) && indent <= sectionIndent && !trimmed.StartsWith(sectionKey))
            {
                if (subKey is null || inSubSection)
                    break; // Einde van de sectie
            }

            if (subKey is not null && !inSubSection)
            {
                if (trimmed.StartsWith(subKey, StringComparison.OrdinalIgnoreCase))
                {
                    inSubSection = true;
                    subSectionIndent = indent;
                    result.Add(trimmed);
                }
                continue;
            }

            if (inSubSection && subKey is not null && !string.IsNullOrWhiteSpace(line) && indent <= subSectionIndent && i > 0)
            {
                var prevTrimmed = lines[i - 1].TrimStart();
                if (!prevTrimmed.StartsWith(subKey, StringComparison.OrdinalIgnoreCase))
                    break;
            }

            result.Add(line);
        }

        return result.Count > 0
            ? string.Join('\n', result).Trim()
            : $"Sectie niet gevonden in het document.";
    }
}
