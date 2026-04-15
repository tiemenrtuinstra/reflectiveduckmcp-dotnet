namespace ReflectieveDuck.Feedback.Domain.ValueObjects;

public readonly record struct FeedbackTag(string Value)
{
    public override string ToString() => Value;

    public static IReadOnlyList<FeedbackTag> Parse(string? tags)
        => string.IsNullOrWhiteSpace(tags)
            ? []
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                  .Select(t => new FeedbackTag(t.ToLowerInvariant()))
                  .ToList();

    public static string Join(IEnumerable<FeedbackTag> tags)
        => string.Join(",", tags.Select(t => t.Value));
}
