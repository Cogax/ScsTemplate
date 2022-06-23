using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Extensions;

public static class MessagingAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITodoMessagingPort, TodoMessagingAdapter>();

        return services;
    }
}
