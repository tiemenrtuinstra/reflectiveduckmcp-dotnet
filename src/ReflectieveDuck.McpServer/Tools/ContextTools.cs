using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Context.Application.DTOs;
using ReflectieveDuck.Context.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class ContextTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<GetPatternsQuery, IReadOnlyList<PatternDto>> _patterns;
    private readonly IQueryHandler<GetInsightsQuery, IReadOnlyList<InsightDto>> _insights;
    private readonly IQueryHandler<GetFullContextQuery, ReflectieContextDto> _fullContext;
    private readonly IQueryHandler<GetDashboardQuery, DashboardDto> _dashboard;

    public ContextTools(
        IQueryHandler<GetPatternsQuery, IReadOnlyList<PatternDto>> patterns,
        IQueryHandler<GetInsightsQuery, IReadOnlyList<InsightDto>> insights,
        IQueryHandler<GetFullContextQuery, ReflectieContextDto> fullContext,
        IQueryHandler<GetDashboardQuery, DashboardDto> dashboard)
    {
        _patterns = patterns;
        _insights = insights;
        _fullContext = fullContext;
        _dashboard = dashboard;
    }

    [McpServerTool(Name = "context_patronen"),
     Description("Analyseer patronen in je feedback en stoplicht-geschiedenis. Toont terugkerende thema's en trends.")]
    public async Task<string> GetPatterns()
    {
        var result = await _patterns.HandleAsync(new GetPatternsQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "context_inzichten"),
     Description("Genereer inzichten op basis van je feedback-data. Toont gemiddelden, trends en observaties.")]
    public async Task<string> GetInsights()
    {
        var result = await _insights.HandleAsync(new GetInsightsQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "context_volledig"),
     Description("Haal de volledige reflectiecontext op: huidige status, patronen, inzichten en statistieken.")]
    public async Task<string> GetFullContext()
    {
        var result = await _fullContext.HandleAsync(new GetFullContextQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "dashboard"),
     Description("Haal het complete dashboard op in één call: stoplichtkleur, energielevel, actieve focus sessie, energie-trend, feedback van vandaag, patronen en gepersonaliseerd advies.")]
    public async Task<string> GetDashboard()
    {
        var result = await _dashboard.HandleAsync(new GetDashboardQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
