using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.Consistency.Outbox;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Consistency;

public static class CommonServiceCollectionExtensions
{
    public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IOutbox, HangfireOutbox>();
        //services.AddScoped<IOutbox, NServiceBusOutbox>();
        services.AddScoped<IPersistenceTransaction, TransactionScopePersistenceTransaction>();
        return services;
    }
}
