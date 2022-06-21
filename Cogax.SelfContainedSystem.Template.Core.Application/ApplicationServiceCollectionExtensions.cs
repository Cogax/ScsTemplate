using Cogax.SelfContainedSystem.Template.Core.Application.Common;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Core.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        services.AddApplicationCommon();

        return services;
    }
}
