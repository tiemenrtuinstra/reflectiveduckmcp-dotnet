namespace ReflectieveDuck.Analyse.Application.DTOs;

public record StacktraceAnalysisDto(
    string FoutType,
    string FoutBericht,
    string? BronBestand,
    int? RegelNummer,
    string Uitleg,
    IReadOnlyList<string> MogelijkeOorzaken,
    IReadOnlyList<string> Suggesties);
