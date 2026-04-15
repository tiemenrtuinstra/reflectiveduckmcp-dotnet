namespace ReflectieveDuck.Resources.Application.DTOs;

public record ConfigSummaryDto(
    int MaxReflectieVragen,
    int FeedbackRetentionDays,
    int OptimalFocusMinutes,
    double MaxMeetingRatio,
    int MaxInterruptionsPerHour,
    int CacheTtlMinutes);
