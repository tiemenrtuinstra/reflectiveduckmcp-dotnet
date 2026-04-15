using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Resources.Application.DTOs;
using ReflectieveDuck.Resources.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class ResourceTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<GetIjsbergMetafoorQuery, string> _ijsberg;
    private readonly IQueryHandler<GetAddendumQuery, string> _addendum;
    private readonly IQueryHandler<GetAssWijzerQuery, string> _assWijzer;
    private readonly IQueryHandler<GetReflectieAssistentQuery, string> _reflectieAssistent;
    private readonly IQueryHandler<GetLifeMapQuery, string> _lifeMap;
    private readonly IQueryHandler<GetStrengthsProfileQuery, string> _strengths;
    private readonly IQueryHandler<GetHealthQuery, HealthDto> _health;
    private readonly IQueryHandler<GetConfigSummaryQuery, ConfigSummaryDto> _config;

    public ResourceTools(
        IQueryHandler<GetIjsbergMetafoorQuery, string> ijsberg,
        IQueryHandler<GetAddendumQuery, string> addendum,
        IQueryHandler<GetAssWijzerQuery, string> assWijzer,
        IQueryHandler<GetReflectieAssistentQuery, string> reflectieAssistent,
        IQueryHandler<GetLifeMapQuery, string> lifeMap,
        IQueryHandler<GetStrengthsProfileQuery, string> strengths,
        IQueryHandler<GetHealthQuery, HealthDto> health,
        IQueryHandler<GetConfigSummaryQuery, ConfigSummaryDto> config)
    {
        _ijsberg = ijsberg;
        _addendum = addendum;
        _assWijzer = assWijzer;
        _reflectieAssistent = reflectieAssistent;
        _lifeMap = lifeMap;
        _strengths = strengths;
        _health = health;
        _config = config;
    }

    [McpServerTool(Name = "resource_ijsberg"),
     Description("Bekijk de IJsberg-Metafoor: wat je ziet vs. wat je niet ziet bij neurodivergent welzijn.")]
    public async Task<string> GetIjsbergMetafoor()
        => await _ijsberg.HandleAsync(new GetIjsbergMetafoorQuery());

    [McpServerTool(Name = "resource_addendum"),
     Description("Bekijk het Addendum met werkstrategieën, wettelijk kader, SCARF, Leary, feedback en rolverdeling.")]
    public async Task<string> GetAddendum()
        => await _addendum.HandleAsync(new GetAddendumQuery());

    [McpServerTool(Name = "resource_asswijzer"),
     Description("Bekijk de ingevulde ASS-wijzer (psycho-educatie): prikkelgevoeligheden, communicatiebehoeften, veranderingen, reacties en wat helpt. Optioneel een sectie: prikkels, communicatie, veranderingen, reactie, zelfhulp, anderen.")]
    public async Task<string> GetAssWijzer(
        [Description("Sectie: prikkels, communicatie, veranderingen, reactie, zelfhulp, anderen (optioneel, zonder = alles)")] string? sectie = null)
        => await _assWijzer.HandleAsync(new GetAssWijzerQuery(sectie));

    [McpServerTool(Name = "resource_reflectie_assistent"),
     Description("Bekijk de handleiding van de AI Reflectie-Assistent: communicatieafspraken, modules (Theory of Mind, Executieve Functies, Sociale Coherentie), dating, werkreflectie, feedbackregels, kernkwadranten en Reflectieve Duck-modus.")]
    public async Task<string> GetReflectieAssistent()
        => await _reflectieAssistent.HandleAsync(new GetReflectieAssistentQuery());

    [McpServerTool(Name = "resource_lifemap"),
     Description("Bekijk de Life Map tijdlijn (1989-2025): vroege jeugd, pesten, scouting, studie, werk, ASS-diagnose, Gilwell, relatie en politiek.")]
    public async Task<string> GetLifeMap()
        => await _lifeMap.HandleAsync(new GetLifeMapQuery());

    [McpServerTool(Name = "resource_sterktes"),
     Description("Bekijk het VIA Character Strengths profiel: dominante karaktersterktes zoals eerlijkheid, rechtvaardigheid, nieuwsgierigheid, moed en betrouwbaarheid.")]
    public async Task<string> GetStrengthsProfile()
        => await _strengths.HandleAsync(new GetStrengthsProfileQuery());

    [McpServerTool(Name = "resource_health"),
     Description("Controleer de gezondheid van de Reflectieve Duck: database status, versie en statistieken.")]
    public async Task<string> GetHealth()
    {
        var result = await _health.HandleAsync(new GetHealthQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "resource_config"),
     Description("Bekijk de huidige configuratie-instellingen van de Reflectieve Duck.")]
    public async Task<string> GetConfig()
    {
        var result = await _config.HandleAsync(new GetConfigSummaryQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
