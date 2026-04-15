using System.Text.RegularExpressions;
using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Domain.Services;

namespace ReflectieveDuck.Analyse.Infrastructure;

public partial class CodeAnalyzer : ICodeAnalyzer
{
    public CodeAnalysisDto Analyze(string code, string? taal = null)
    {
        var regels = code.Split('\n');
        var aantalRegels = regels.Length;
        var aantalMethodes = MethodeRegex().Matches(code).Count;

        var suggesties = new List<string>();

        if (aantalRegels > 100)
            suggesties.Add("Overweeg de code op te splitsen in kleinere functies.");
        if (aantalMethodes > 10)
            suggesties.Add("Veel methodes in één bestand — overweeg aparte klassen.");

        var nestingDepth = regels.Max(r => r.TakeWhile(c => c == ' ').Count() / 4);
        if (nestingDepth > 4)
            suggesties.Add("Diepe nesting gedetecteerd — overweeg early returns of guard clauses.");

        var longLines = regels.Count(r => r.Length > 120);
        if (longLines > 5)
            suggesties.Add($"{longLines} regels zijn langer dan 120 tekens.");

        var commentLines = regels.Count(r => r.TrimStart().StartsWith("//") || r.TrimStart().StartsWith("#"));
        if (commentLines == 0 && aantalRegels > 20)
            suggesties.Add("Geen comments gevonden — overweeg documentatie toe te voegen.");

        var complexiteit = (aantalRegels, aantalMethodes) switch
        {
            ( < 30, _) => "Laag",
            ( < 100, < 5) => "Gemiddeld",
            ( < 100, _) => "Gemiddeld-hoog",
            _ => "Hoog"
        };

        var leesbaarheid = (nestingDepth, longLines) switch
        {
            ( <= 2, <= 2) => "Goed",
            ( <= 3, <= 5) => "Redelijk",
            _ => "Kan beter"
        };

        if (suggesties.Count == 0)
            suggesties.Add("Code ziet er goed uit! Geen directe verbeterpunten.");

        return new CodeAnalysisDto(aantalRegels, aantalMethodes, complexiteit, suggesties, leesbaarheid);
    }

    [GeneratedRegex(@"(public|private|protected|internal)?\s*(static\s+)?(async\s+)?\w+\s+\w+\s*\(")]
    private static partial Regex MethodeRegex();
}
