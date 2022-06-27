using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public static class ExecutionContextServiceCollectionExtensions
{
    public static IServiceCollection AddExecutionContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DefaultExecutionContext>();
        services.AddScoped<NServiceBusMessageHandlerExecutionContext>();
        services.AddScoped<IExecutionContextFactory, ExecutionContextFactory>();
        services.AddScoped<IExecutionContext>(sp => sp.GetRequiredService<IExecutionContextFactory>().Create());

        return services;
    }
}
