using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class FeedbackTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<AddFeedbackCommand, FeedbackDto> _add;
    private readonly IQueryHandler<GetFeedbackListQuery, IReadOnlyList<FeedbackDto>> _list;
    private readonly IQueryHandler<SearchFeedbackQuery, IReadOnlyList<FeedbackDto>> _search;
    private readonly IQueryHandler<GetFeedbackStatsQuery, FeedbackStatsDto> _stats;
    private readonly IQueryHandler<GetFeedbackTagsQuery, IReadOnlyList<TagFrequentie>> _tags;

    public FeedbackTools(
        IQueryHandler<AddFeedbackCommand, FeedbackDto> add,
        IQueryHandler<GetFeedbackListQuery, IReadOnlyList<FeedbackDto>> list,
        IQueryHandler<SearchFeedbackQuery, IReadOnlyList<FeedbackDto>> search,
        IQueryHandler<GetFeedbackStatsQuery, FeedbackStatsDto> stats,
        IQueryHandler<GetFeedbackTagsQuery, IReadOnlyList<TagFrequentie>> tags)
    {
        _add = add;
        _list = list;
        _search = search;
        _stats = stats;
        _tags = tags;
    }

    [McpServerTool(Name = "feedback_toevoegen"),
     Description("Voeg een feedback-entry toe. Leg vast wat je hebt meegemaakt, hoe je je voelde, en wat je ervan geleerd hebt.")]
    public async Task<string> AddFeedback(
        [Description("Onderwerp van de feedback")] string onderwerp,
        [Description("Hoe voelde je je? (optioneel)")] string? gevoel = null,
        [Description("Welk inzicht heb je opgedaan? (optioneel)")] string? inzicht = null,
        [Description("Tags, komma-gescheiden (optioneel)")] string? tags = null,
        [Description("Energielevel 0-100 (optioneel)")] int? energieLevel = null,
        [Description("Stoplicht kleur: groen/oranje/rood/blauw (optioneel)")] string? stoplichtStatus = null,
        [Description("Categorie (optioneel)")] string? categorie = null)
    {
        var result = await _add.HandleAsync(new AddFeedbackCommand(
            onderwerp, gevoel, inzicht, tags, energieLevel, stoplichtStatus, categorie));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "feedback_lijst"),
     Description("Bekijk recente feedback-entries.")]
    public async Task<string> ListFeedback(
        [Description("Aantal entries om te tonen (standaard 20)")] int limit = 20)
    {
        var result = await _list.HandleAsync(new GetFeedbackListQuery(limit));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "feedback_zoeken"),
     Description("Zoek in feedback-entries op onderwerp, gevoel of inzicht.")]
    public async Task<string> SearchFeedback(
        [Description("Zoekterm")] string zoekterm)
    {
        var result = await _search.HandleAsync(new SearchFeedbackQuery(zoekterm));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "feedback_statistieken"),
     Description("Bekijk statistieken over je feedback: top tags, categorieën, gemiddeld energielevel en meest voorkomend gevoel.")]
    public async Task<string> GetStats()
    {
        var result = await _stats.HandleAsync(new GetFeedbackStatsQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "feedback_tags"),
     Description("Bekijk alle gebruikte tags en hoe vaak ze voorkomen.")]
    public async Task<string> GetTags()
    {
        var result = await _tags.HandleAsync(new GetFeedbackTagsQuery());
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
