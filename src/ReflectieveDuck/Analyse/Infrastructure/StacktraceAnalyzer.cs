using System.Text.RegularExpressions;
using ReflectieveDuck.Analyse.Application.DTOs;
using ReflectieveDuck.Analyse.Domain.Services;

namespace ReflectieveDuck.Analyse.Infrastructure;

public partial class StacktraceAnalyzer : IStacktraceAnalyzer
{
    public StacktraceAnalysisDto Analyze(string stacktrace)
    {
        var lines = stacktrace.Split('\n').Select(l => l.Trim()).ToArray();

        // Extract exception type and message
        var foutType = "Onbekend";
        var foutBericht = "Kon het foutbericht niet extraheren.";
        string? bronBestand = null;
        int? regelNummer = null;

        // Try .NET format: "System.NullReferenceException: Object reference..."
        var exceptionMatch = ExceptionRegex().Match(lines[0]);
        if (exceptionMatch.Success)
        {
            foutType = exceptionMatch.Groups[1].Value;
            foutBericht = exceptionMatch.Groups[2].Value;
        }
        // Try Python format: "TypeError: cannot..."
        else
        {
            var pythonMatch = PythonExceptionRegex().Match(lines[^1]);
            if (pythonMatch.Success)
            {
                foutType = pythonMatch.Groups[1].Value;
                foutBericht = pythonMatch.Groups[2].Value;
            }
        }

        // Extract file and line from stack frames
        foreach (var line in lines)
        {
            var fileMatch = FileLineRegex().Match(line);
            if (fileMatch.Success)
            {
                bronBestand = fileMatch.Groups[1].Value;
                if (int.TryParse(fileMatch.Groups[2].Value, out var ln))
                    regelNummer = ln;
                break;
            }
        }

        var uitleg = GenereerUitleg(foutType);
        var oorzaken = GenereerOorzaken(foutType);
        var suggesties = GenereerSuggesties(foutType);

        return new StacktraceAnalysisDto(
            foutType, foutBericht, bronBestand, regelNummer,
            uitleg, oorzaken, suggesties);
    }

    private static string GenereerUitleg(string foutType) => foutType switch
    {
        var t when t.Contains("NullReference") =>
            "Er wordt geprobeerd een methode of eigenschap aan te roepen op een object dat null is.",
        var t when t.Contains("ArgumentNull") =>
            "Een methode heeft een null-argument ontvangen terwijl dat niet is toegestaan.",
        var t when t.Contains("IndexOutOfRange") =>
            "Er wordt geprobeerd een element op te halen met een index die buiten het bereik van de collectie valt.",
        var t when t.Contains("InvalidOperation") =>
            "Een bewerking is niet geldig voor de huidige status van het object.",
        var t when t.Contains("FileNotFound") =>
            "Het opgegeven bestand kon niet worden gevonden op het verwachte pad.",
        var t when t.Contains("Timeout") =>
            "Een bewerking heeft te lang geduurd en is verlopen.",
        var t when t.Contains("TypeError") =>
            "Een waarde heeft niet het verwachte type voor de bewerking.",
        var t when t.Contains("KeyError") =>
            "Een sleutel is niet gevonden in een dictionary/map.",
        _ => $"Er is een {foutType} opgetreden. Controleer de stacktrace voor meer details."
    };

    private static IReadOnlyList<string> GenereerOorzaken(string foutType) => foutType switch
    {
        var t when t.Contains("NullReference") =>
        [
            "Object niet geïnitialiseerd voor gebruik",
            "Database query retourneert null",
            "Optionele parameter niet gecontroleerd"
        ],
        var t when t.Contains("IndexOutOfRange") =>
        [
            "Loop gaat voorbij de array-lengte",
            "Off-by-one error in index berekening",
            "Lege collectie zonder controle"
        ],
        var t when t.Contains("Timeout") =>
        [
            "Netwerkverbinding traag of verbroken",
            "Database query te complex",
            "Deadlock in concurrent code"
        ],
        _ =>
        [
            "Onverwachte invoer of status",
            "Configuratiefout",
            "Race condition of timing probleem"
        ]
    };

    private static IReadOnlyList<string> GenereerSuggesties(string foutType) => foutType switch
    {
        var t when t.Contains("NullReference") =>
        [
            "Gebruik null-conditional operators (?. en ??)",
            "Voeg null-checks toe voor de aanroep",
            "Overweeg het Null Object pattern"
        ],
        var t when t.Contains("IndexOutOfRange") =>
        [
            "Controleer .Count/.Length voor toegang",
            "Gebruik .FirstOrDefault() i.p.v. [0]",
            "Gebruik een foreach loop i.p.v. index-based"
        ],
        var t when t.Contains("Timeout") =>
        [
            "Verhoog de timeout-waarde als dat gepast is",
            "Voeg retry-logica toe met exponential backoff",
            "Optimaliseer de onderliggende query/bewerking"
        ],
        _ =>
        [
            "Voeg meer specifieke error handling toe",
            "Log de volledige exception voor debugging",
            "Voeg unit tests toe voor dit scenario"
        ]
    };

    [GeneratedRegex(@"^([\w.]+Exception):\s*(.+)$")]
    private static partial Regex ExceptionRegex();

    [GeneratedRegex(@"^(\w+Error):\s*(.+)$")]
    private static partial Regex PythonExceptionRegex();

    [GeneratedRegex(@"(?:in|at|File)\s+[""']?(.+?)[""']?\s*(?::line\s+|, line\s+|:)(\d+)")]
    private static partial Regex FileLineRegex();
}
