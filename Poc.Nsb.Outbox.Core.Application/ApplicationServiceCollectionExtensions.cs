using Microsoft.Extensions.DependencyInjection;

using Poc.Nsb.Outbox.Core.Application.Common;

namespace Poc.Nsb.Outbox.Core.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddCoreApplication(this IServiceCollection services)
    {
        services.AddApplicationCommon();

        return services;
    }
}
