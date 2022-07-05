using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Email;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Infrastructure.Consistency;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommonInfrastructure();services.AddPersistenceAdapter(configuration);

        services.AddSignalRAdapter(configuration);
        services.AddMessagingAdapter(configuration);
        services.AddHangfireAdapter(configuration);
        services.AddEmailAdapter(configuration);
        
        return services;
    }
}
