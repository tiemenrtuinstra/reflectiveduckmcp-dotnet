namespace ReflectieveDuck.Analyse.Application.DTOs;

public record CodeAnalysisDto(
    int AantalRegels,
    int AantalMethodes,
    string ComplexiteitsNiveau,
    IReadOnlyList<string> Suggesties,
    string LeesbaarheidsScore);
