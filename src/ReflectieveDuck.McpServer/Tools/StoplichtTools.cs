using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Shared.Application;
using ReflectieveDuck.Stoplichtplan.Application.DTOs;
using ReflectieveDuck.Stoplichtplan.Application.Queries;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class StoplichtTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<GetCurrentStatusQuery, StoplichtStatusDto?> _getCurrent;
    private readonly IQueryHandler<GetStatusDetailQuery, IReadOnlyList<StoplichtStatusDto>> _getDetail;
    private readonly IQueryHandler<UpdateStatusCommand, StoplichtStatusDto> _update;
    private readonly IQueryHandler<CompareStatusesQuery, StoplichtVergelijkingDto?> _compare;

    public StoplichtTools(
        IQueryHandler<GetCurrentStatusQuery, StoplichtStatusDto?> getCurrent,
        IQueryHandler<GetStatusDetailQuery, IReadOnlyList<StoplichtStatusDto>> getDetail,
        IQueryHandler<UpdateStatusCommand, StoplichtStatusDto> update,
        IQueryHandler<CompareStatusesQuery, StoplichtVergelijkingDto?> compare)
    {
        _getCurrent = getCurrent;
        _getDetail = getDetail;
        _update = update;
        _compare = compare;
    }

    [McpServerTool(Name = "stoplicht_status"),
     Description("Haal de huidige stoplicht-status op (groen/oranje/rood/blauw). Toont energielevel en kleur.")]
    public async Task<string> GetStatus()
    {
        var result = await _getCurrent.HandleAsync(new GetCurrentStatusQuery());
        if (result is null)
            return "Nog geen stoplicht-status geregistreerd. Gebruik 'stoplicht_update' om je eerste status in te voeren.";
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "stoplicht_geschiedenis"),
     Description("Bekijk de geschiedenis van stoplicht-statussen. Toont trends over tijd.")]
    public async Task<string> GetHistory(
        [Description("Aantal statussen om te tonen (standaard 10)")] int limit = 10)
    {
        var result = await _getDetail.HandleAsync(new GetStatusDetailQuery(limit));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "stoplicht_update"),
     Description("Werk de stoplicht-status bij. Kies een kleur (groen/oranje/rood/blauw) en energielevel (0-100).")]
    public async Task<string> UpdateStatus(
        [Description("Kleur: groen, oranje, rood of blauw")] string kleur,
        [Description("Energielevel van 0 tot 100")] int energieLevel,
        [Description("Optionele notities bij deze status")] string? notities = null)
    {
        var result = await _update.HandleAsync(
            new UpdateStatusCommand(kleur, energieLevel, notities));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "stoplicht_vergelijk"),
     Description("Vergelijk twee stoplicht-statussen om trends te zien. Zonder parameters vergelijkt de laatste twee.")]
    public async Task<string> CompareStatuses(
        [Description("ID van de vorige status (optioneel)")] int? vorigeId = null,
        [Description("ID van de huidige status (optioneel)")] int? huidigeId = null)
    {
        var result = await _compare.HandleAsync(
            new CompareStatusesQuery(vorigeId, huidigeId));
        if (result is null)
            return "Niet genoeg statussen om te vergelijken. Registreer minimaal twee statussen.";
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
