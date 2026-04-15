using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Domain.Entities;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Feedback.Domain.ValueObjects;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback.Application.Queries;

public record AddFeedbackCommand(
    string Onderwerp,
    string? Gevoel = null,
    string? Inzicht = null,
    string? Tags = null,
    int? EnergieLevel = null,
    string? StoplichtStatus = null,
    string? Categorie = null);

public class AddFeedbackCommandHandler
    : IQueryHandler<AddFeedbackCommand, FeedbackDto>
{
    private readonly IFeedbackRepository _repo;

    public AddFeedbackCommandHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<FeedbackDto> HandleAsync(
        AddFeedbackCommand command, CancellationToken ct = default)
    {
        var tags = FeedbackTag.Parse(command.Tags);

        var entry = new FeedbackEntry
        {
            Onderwerp = command.Onderwerp,
            Gevoel = command.Gevoel,
            Inzicht = command.Inzicht,
            Tags = tags.Count > 0 ? FeedbackTag.Join(tags) : null,
            EnergieLevel = command.EnergieLevel,
            StoplichtStatus = command.StoplichtStatus,
            Categorie = command.Categorie
        };

        var saved = await _repo.AddAsync(entry, ct);
        return ToDto(saved);
    }

    internal static FeedbackDto ToDto(FeedbackEntry e)
        => new(e.Id, e.Onderwerp, e.Gevoel, e.Inzicht,
            FeedbackTag.Parse(e.Tags).Select(t => t.Value).ToList(),
            e.EnergieLevel, e.StoplichtStatus, e.Categorie, e.CreatedAt);
}
