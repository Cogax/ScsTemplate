using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ReadmodelProviders;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;

public static class PersistenceAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WriteModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(configuration["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600)));

        services.AddDbContext<ReadModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(configuration["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<ITodoReadModelProvider, TodoReadmodelProvider>();
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IPersistenceAdapter, SqlServerPersistenceAdapter>();

        return services;
    }
}
