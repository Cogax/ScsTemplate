using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire.Recurring;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Hangfire;
using Hangfire.SqlServer;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public static class HangfireAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            DashboardJobListLimit = 5000,
            JobExpirationCheckInterval = TimeSpan.FromMinutes(30),
        });
        
        services.AddHangfire((sp, options) => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration["ConnectionStrings:Db"], sp.GetRequiredService<SqlServerStorageOptions>()));

        services.AddHostedService<RecurringJobInitializationBackgroundService>();

        services.Replace(new ServiceDescriptor(typeof(IBackgroundJobClient), sp =>
                new BackgroundJobClient(new SqlServerStorage(
                    sp.GetRequiredService<WriteModelDbContext>().Database.GetDbConnection(),
                    sp.GetRequiredService<SqlServerStorageOptions>())),
            ServiceLifetime.Scoped));

        return services;
    }

    public static IServiceCollection AddHangfireServerAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfireServer();

        return services;
    }
}
