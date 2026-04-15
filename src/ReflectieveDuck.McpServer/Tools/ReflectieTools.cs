using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Reflectie.Application.DTOs;
using ReflectieveDuck.Reflectie.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class ReflectieTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<GenerateReflectieQuery, IReadOnlyList<ReflectieVraagDto>> _generate;

    public ReflectieTools(
        IQueryHandler<GenerateReflectieQuery, IReadOnlyList<ReflectieVraagDto>> generate)
        => _generate = generate;

    [McpServerTool(Name = "reflectie_vragen"),
     Description("Genereer reflectievragen afgestemd op je huidige stoplicht-kleur en energielevel. Helpt je om bewust stil te staan bij hoe het gaat.")]
    public async Task<string> GenerateVragen(
        [Description("Stoplicht kleur: groen/oranje/rood/blauw (optioneel, past vragen aan)")] string? stoplichtKleur = null,
        [Description("Energielevel 0-100 (optioneel)")] int? energieLevel = null,
        [Description("Aantal vragen (standaard 5)")] int maxVragen = 5)
    {
        var result = await _generate.HandleAsync(
            new GenerateReflectieQuery(stoplichtKleur, energieLevel, maxVragen));
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
