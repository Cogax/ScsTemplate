using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Hangfire;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public static class HangfireAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddHangfireAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var sqlServerStorageOptions = new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            DashboardJobListLimit = 5000,
            JobExpirationCheckInterval = TimeSpan.FromMinutes(30),
        };

        services.AddSingleton(sqlServerStorageOptions);

        services.AddHangfire(options => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration["ConnectionStrings:Db"], sqlServerStorageOptions));

        return services;
    }

    public static IServiceCollection AddHangfireServerAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection AddHangfireOutboxAdapter(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        var nsbMessageSessionImplementationFactory = services
            .FirstOrDefault(d => d.ServiceType == typeof(IMessageSession))?
            .ImplementationFactory;

        if (nsbMessageSessionImplementationFactory == null)
            throw new Exception("Hangfire Outbox must be registered after NServiceBus!");

        services.Replace(new ServiceDescriptor(typeof(IBackgroundJobClient), sp =>
            new BackgroundJobClient(new SqlServerStorage(
                sp.GetRequiredService<WriteModelDbContext>().Database.GetDbConnection(),
                sp.GetRequiredService<SqlServerStorageOptions>())),
            ServiceLifetime.Scoped));

        services.AddScoped<HangfireOutboxMessageSession>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Replace(new ServiceDescriptor(typeof(IMessageSession), sp =>
        {
            if (sp.GetService<IHttpContextAccessor>()?.HttpContext == null)
                return nsbMessageSessionImplementationFactory(sp);

            return sp.GetRequiredService<HangfireOutboxMessageSession>();
        }, ServiceLifetime.Scoped));

        return services;
    }
}
