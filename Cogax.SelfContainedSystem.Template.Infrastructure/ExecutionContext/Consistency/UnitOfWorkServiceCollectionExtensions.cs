using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency;

public static class UnitOfWorkServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TransactionScopePersistenceTransaction>();
        services.AddScoped<DefaultPersistenceTransaction>();

        return services;
    }
}
