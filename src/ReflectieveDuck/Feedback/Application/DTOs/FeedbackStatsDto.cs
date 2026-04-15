namespace ReflectieveDuck.Feedback.Application.DTOs;

public record FeedbackStatsDto(
    int TotaalAantal,
    IReadOnlyList<TagFrequentie> TopTags,
    IReadOnlyList<CategorieFrequentie> TopCategorieen,
    double? GemiddeldEnergieLevel,
    string? MeestVoorkomendGevoel);

public record TagFrequentie(string Tag, int Aantal);
public record CategorieFrequentie(string Categorie, int Aantal);
