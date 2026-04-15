namespace ReflectieveDuck.Stoplichtplan.Application.DTOs;

public record StoplichtVergelijkingDto(
    StoplichtStatusDto Vorige,
    StoplichtStatusDto Huidige,
    string Trend,
    int EnergieVerschil);
