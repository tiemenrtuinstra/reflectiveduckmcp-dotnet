using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Feedback.Domain.ValueObjects;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback.Application.Queries;

public record GetFeedbackTagsQuery;

public class GetFeedbackTagsQueryHandler
    : IQueryHandler<GetFeedbackTagsQuery, IReadOnlyList<TagFrequentie>>
{
    private readonly IFeedbackRepository _repo;

    public GetFeedbackTagsQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<TagFrequentie>> HandleAsync(
        GetFeedbackTagsQuery query, CancellationToken ct = default)
    {
        var all = await _repo.GetAllAsync(limit: 1000, ct);

        return all
            .Where(f => f.Tags is not null)
            .SelectMany(f => FeedbackTag.Parse(f.Tags))
            .GroupBy(t => t.Value)
            .OrderByDescending(g => g.Count())
            .Select(g => new TagFrequentie(g.Key, g.Count()))
            .ToList();
    }
}
