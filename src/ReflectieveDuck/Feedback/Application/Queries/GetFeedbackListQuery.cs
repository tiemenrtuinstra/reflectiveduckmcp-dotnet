using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback.Application.Queries;

public record GetFeedbackListQuery(int Limit = 20);

public class GetFeedbackListQueryHandler
    : IQueryHandler<GetFeedbackListQuery, IReadOnlyList<FeedbackDto>>
{
    private readonly IFeedbackRepository _repo;

    public GetFeedbackListQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<FeedbackDto>> HandleAsync(
        GetFeedbackListQuery query, CancellationToken ct = default)
    {
        var entries = await _repo.GetAllAsync(query.Limit, ct);
        return entries.Select(AddFeedbackCommandHandler.ToDto).ToList();
    }
}
