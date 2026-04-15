namespace ReflectieveDuck.Shared.Infrastructure.Configuration;

public sealed class ReflectieOptions
{
    public const string SectionName = "Reflectie";

    public int MaxReflectieVragen { get; init; } = 5;
    public int FeedbackRetentionDays { get; init; } = 365;
    public int OptimalFocusMinutes { get; init; } = 90;
    public double MaxMeetingRatio { get; init; } = 0.40;
    public int MaxInterruptionsPerHour { get; init; } = 2;
}
