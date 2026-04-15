using ReflectieveDuck.Feedback.Domain.Entities;

namespace ReflectieveDuck.Feedback.Domain.Repositories;

public interface IFeedbackRepository
{
    Task<FeedbackEntry> AddAsync(FeedbackEntry entry, CancellationToken ct = default);
    Task<IReadOnlyList<FeedbackEntry>> GetAllAsync(int limit = 20, CancellationToken ct = default);
    Task<IReadOnlyList<FeedbackEntry>> SearchAsync(string zoekterm, CancellationToken ct = default);
    Task<IReadOnlyList<FeedbackEntry>> GetByTagAsync(string tag, CancellationToken ct = default);
}
