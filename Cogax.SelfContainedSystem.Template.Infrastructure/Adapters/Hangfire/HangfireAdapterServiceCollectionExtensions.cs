using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

using Hangfire;
using Hangfire.Client;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NServiceBus;
using NServiceBus.UniformSession;

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
        
        services.AddHangfire((sp, options) => options
            .UseActivator(new HangfireJobActivator(sp.GetRequiredService<IServiceScopeFactory>()))
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

        var nsbUniformSessionImplementationFactory = services
            .FirstOrDefault(d => d.ServiceType == typeof(IUniformSession))?
            .ImplementationFactory;

        if (nsbMessageSessionImplementationFactory == null ||
            nsbUniformSessionImplementationFactory == null)
            throw new Exception("Hangfire Outbox needs to be registered after NServiceBus and EnableUniformSession() needs" +
                                "to be configured on the EndpointConfiguration (requires NServiceBus.UniformSession package)!");

        services.Replace(new ServiceDescriptor(typeof(IBackgroundJobClient), sp =>
            new BackgroundJobClient(new SqlServerStorage(
                sp.GetRequiredService<WriteModelDbContext>().Database.GetDbConnection(),
                sp.GetRequiredService<SqlServerStorageOptions>())),
            ServiceLifetime.Scoped));

        services.AddScoped<HangfireOutboxUniformSession>();

        Func<IServiceProvider, object> messageSessionFactory = (sp) =>
        {
            var executionContext = sp.GetRequiredService<IExecutionContext>();
            var outboxType = executionContext.GetExecutionContextOutboxType();

            switch (outboxType)
            {
                case ExecutionContextOutboxType.NServiceBusOutbox:
                    return nsbUniformSessionImplementationFactory(sp);
                case ExecutionContextOutboxType.HangfireOutbox:
                    return sp.GetRequiredService<HangfireOutboxUniformSession>();
                default:
                    throw new NotImplementedException(
                        $"{nameof(ExecutionContextOutboxType)} '{outboxType}' not implemented!");
            }
        };

        services.Replace(new ServiceDescriptor(typeof(IMessageSession), sp =>
            throw new Exception("Use IUniformSession instead of IMessageSession!"), ServiceLifetime.Scoped));
        services.Replace(new ServiceDescriptor(typeof(IUniformSession), messageSessionFactory, ServiceLifetime.Scoped));
        
        return services;
    }
}
