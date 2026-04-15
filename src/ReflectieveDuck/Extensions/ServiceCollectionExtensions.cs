using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReflectieveDuck.Shared.Infrastructure.Configuration;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;
using ReflectieveDuck.Analyse;
using ReflectieveDuck.Context;
using ReflectieveDuck.Feedback;
using ReflectieveDuck.Productiviteit;
using ReflectieveDuck.Reflectie;
using ReflectieveDuck.Resources;
using ReflectieveDuck.Stoplichtplan;

namespace ReflectieveDuck.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReflectieveDuck(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options
        services.Configure<ReflectieOptions>(
            configuration.GetSection(ReflectieOptions.SectionName));
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SectionName));

        // Database
        var dbPath = Environment.GetEnvironmentVariable("DB_PATH")
            ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ReflectieveDuck",
                "local.db");

        // MED-6 fix: null-safe directory aanmaak
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        services.AddDbContext<LocalDbContext>(opts =>
            opts.UseSqlite($"Data Source={dbPath}"));

        // Memory cache
        services.AddMemoryCache();

        // Bounded contexts
        services.AddStoplichtplan();
        services.AddFeedback();
        services.AddReflectie();
        services.AddAnalyse();
        services.AddContext();
        services.AddResources();
        services.AddProductiviteit();

        return services;
    }
}
