using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Productiviteit.Application.DTOs;
using ReflectieveDuck.Productiviteit.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class ProductiviteitTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<StartFocusCommand, FocusSessionDto> _start;
    private readonly IQueryHandler<EndFocusCommand, FocusSessionDto?> _end;
    private readonly IQueryHandler<LogEnergyCommand, EnergyLogDto> _logEnergy;
    private readonly IQueryHandler<AnalyzeProductivityQuery, ProductivityReportDto> _analyze;

    public ProductiviteitTools(
        IQueryHandler<StartFocusCommand, FocusSessionDto> start,
        IQueryHandler<EndFocusCommand, FocusSessionDto?> end,
        IQueryHandler<LogEnergyCommand, EnergyLogDto> logEnergy,
        IQueryHandler<AnalyzeProductivityQuery, ProductivityReportDto> analyze)
    {
        _start = start;
        _end = end;
        _logEnergy = logEnergy;
        _analyze = analyze;
    }

    [McpServerTool(Name = "focus_start"),
     Description("Start een focus sessie. Kies een taak en type (DeepFocus, LightFocus, Meeting, Break).")]
    public async Task<string> StartFocus(
        [Description("De taak waar je aan werkt")] string taak,
        [Description("Type sessie: DeepFocus, LightFocus, Meeting of Break (standaard DeepFocus)")] string state = "DeepFocus")
    {
        var result = await _start.HandleAsync(new StartFocusCommand(taak, state));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "focus_stop"),
     Description("Stop de actieve focus sessie. Optioneel met notities over wat je hebt bereikt.")]
    public async Task<string> EndFocus(
        [Description("ID van de sessie (optioneel, stopt anders de actieve sessie)")] int? id = null,
        [Description("Notities over de sessie (optioneel)")] string? notities = null)
    {
        var result = await _end.HandleAsync(new EndFocusCommand(id, notities));
        if (result is null)
            return "Geen actieve focus sessie gevonden.";
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "energie_log"),
     Description("Log je huidige energielevel (0-100). Wordt automatisch gekoppeld aan een stoplichtkleur.")]
    public async Task<string> LogEnergy(
        [Description("Energielevel van 0 tot 100")] int level)
    {
        var result = await _logEnergy.HandleAsync(new LogEnergyCommand(level));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "productiviteit_analyse"),
     Description("Analyseer je productiviteit over de afgelopen dagen. Toont sessieduur, patronen en advies.")]
    public async Task<string> AnalyzeProductivity(
        [Description("Aantal dagen terug om te analyseren (standaard 7)")] int dagenTerug = 7)
    {
        var result = await _analyze.HandleAsync(new AnalyzeProductivityQuery(dagenTerug));
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
