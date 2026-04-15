using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Feedback.Application.DTOs;
using ReflectieveDuck.Feedback.Application.Queries;
using ReflectieveDuck.Feedback.Domain.Repositories;
using ReflectieveDuck.Feedback.Infrastructure;
using ReflectieveDuck.Shared.Application;

namespace ReflectieveDuck.Feedback;

public static class FeedbackServiceExtensions
{
    public static IServiceCollection AddFeedback(this IServiceCollection services)
    {
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();

        services.AddScoped<IQueryHandler<AddFeedbackCommand, FeedbackDto>,
            AddFeedbackCommandHandler>();

        services.AddScoped<IQueryHandler<GetFeedbackListQuery, IReadOnlyList<FeedbackDto>>,
            GetFeedbackListQueryHandler>();

        services.AddScoped<IQueryHandler<SearchFeedbackQuery, IReadOnlyList<FeedbackDto>>,
            SearchFeedbackQueryHandler>();

        services.AddScoped<IQueryHandler<GetFeedbackStatsQuery, FeedbackStatsDto>,
            GetFeedbackStatsQueryHandler>();

        services.AddScoped<IQueryHandler<GetFeedbackTagsQuery, IReadOnlyList<TagFrequentie>>,
            GetFeedbackTagsQueryHandler>();

        return services;
    }
}
