using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Email;

public static class EmailAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddEmailAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITodoEmailPort, NullTodoEmailAdapter>();

        return services;
    }
}
