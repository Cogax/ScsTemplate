using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<WriteModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(config["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600)));

        services.AddDbContext<ReadModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(config["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        return services;
    }
}
