using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NServiceBus;
using NServiceBus.UniformSession;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public static class ExecutionContextServiceCollectionExtensions
{
    public static IServiceCollection AddExecutionContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DefaultExecutionContext>();
        services.AddScoped<HangfireJobExecutionContext>();
        services.AddScoped<HangfireOutboxJobExecutionContext>();
        services.AddScoped<NServiceBusMessageHandlerExecutionContext>();
        services.AddScoped<ExecutionContextFactory>();
        services.AddScoped<IExecutionContext>(sp => sp.GetRequiredService<ExecutionContextFactory>().Create());

        return services;
    }
}
