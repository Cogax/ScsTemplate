using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Common;

namespace Poc.Nsb.Outbox.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<WriteModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(config["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600)));

        return services;
    }
}
