using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency.Outbox;

public static class OutboxServiceCollectionExtensions
{
    public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<NServiceBusOutbox>();
        services.AddTransient<HangfireOutbox>();
        services.AddScoped<IOutbox>(sp => sp.GetRequiredService<IExecutionContext>().CreateOutbox());

        return services;
    }
}
