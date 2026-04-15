namespace ReflectieveDuck.Shared.Infrastructure.Configuration;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public int FeedbackTtlMinutes { get; init; } = 5;
    public int StoplichtTtlMinutes { get; init; } = 5;
    public int AgendaTtlMinutes { get; init; } = 5;
}
