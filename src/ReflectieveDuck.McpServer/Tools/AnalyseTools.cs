using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;
using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Application.Queries;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.McpServer.Tools;

[McpServerToolType, Authorize]
public class AnalyseTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly IQueryHandler<AnalyzeCodeQuery, CodeAnalysisDto> _codeAnalyzer;
    private readonly IQueryHandler<AnalyzeStacktraceQuery, StacktraceAnalysisDto> _stacktraceAnalyzer;

    public AnalyseTools(
        IQueryHandler<AnalyzeCodeQuery, CodeAnalysisDto> codeAnalyzer,
        IQueryHandler<AnalyzeStacktraceQuery, StacktraceAnalysisDto> stacktraceAnalyzer)
    {
        _codeAnalyzer = codeAnalyzer;
        _stacktraceAnalyzer = stacktraceAnalyzer;
    }

    [McpServerTool(Name = "code_analyse"),
     Description("Analyseer een stuk code op complexiteit, leesbaarheid en geef verbetrsuggesties.")]
    public async Task<string> AnalyzeCode(
        [Description("De code om te analyseren")] string code,
        [Description("Programmeertaal (optioneel, bijv. csharp, python)")] string? taal = null)
    {
        var result = await _codeAnalyzer.HandleAsync(new AnalyzeCodeQuery(code, taal));
        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [McpServerTool(Name = "stacktrace_analyse"),
     Description("Analyseer een stacktrace/foutmelding en krijg een begrijpelijke uitleg met mogelijke oplossingen.")]
    public async Task<string> AnalyzeStacktrace(
        [Description("De stacktrace of foutmelding om te analyseren")] string stacktrace)
    {
        var result = await _stacktraceAnalyzer.HandleAsync(new AnalyzeStacktraceQuery(stacktrace));
        return JsonSerializer.Serialize(result, JsonOptions);
    }
}
