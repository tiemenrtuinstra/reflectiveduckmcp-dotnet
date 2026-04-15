namespace ReflectieveDuck.Productiviteit.Application.DTOs;

public record ProductivityReportDto(
    int TotaalSessies,
    int TotaalMinuten,
    double GemiddeldeSessieDuur,
    string MeestProductieveState,
    IReadOnlyList<FocusSessionDto> RecenteSessies,
    string? Advies);
