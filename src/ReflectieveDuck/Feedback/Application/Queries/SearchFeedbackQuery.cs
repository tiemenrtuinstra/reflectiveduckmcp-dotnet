using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback.Application.Queries;

public record SearchFeedbackQuery(string Zoekterm);

public class SearchFeedbackQueryHandler
    : IQueryHandler<SearchFeedbackQuery, IReadOnlyList<FeedbackDto>>
{
    private readonly IFeedbackRepository _repo;

    public SearchFeedbackQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<FeedbackDto>> HandleAsync(
        SearchFeedbackQuery query, CancellationToken ct = default)
    {
        var entries = await _repo.SearchAsync(query.Zoekterm, ct);
        return entries.Select(AddFeedbackCommandHandler.ToDto).ToList();
    }
}
