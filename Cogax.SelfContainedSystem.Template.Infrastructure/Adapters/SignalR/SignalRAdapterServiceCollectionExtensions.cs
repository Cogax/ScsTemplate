using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

public static class SignalRAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddSignalRAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISignalRPublisher, NullSignalRPublisher>();

        return services;
    }
}
