using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Email;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Outbox;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.UnitOfWork;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistenceAdapter(configuration);
        services.AddSignalRAdapter(configuration);
        services.AddMessagingAdapter(configuration);
        services.AddHangfireAdapter(configuration);
        services.AddOutbox(configuration);
        services.AddEmailAdapter(configuration);

        services.AddExecutionContext(configuration);
        services.AddUnitOfWork(configuration);

        return services;
    }
}
