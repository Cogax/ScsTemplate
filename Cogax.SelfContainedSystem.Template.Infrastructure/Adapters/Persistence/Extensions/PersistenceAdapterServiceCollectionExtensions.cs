using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ReadmodelProviders;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Repositories;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NServiceBus.Persistence.Sql;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;

public static class PersistenceAdapterServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceAdapter(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DbContextOptions<WriteModelDbContext>>(sp =>
        {
            var executionContext = sp.GetRequiredService<IExecutionContext>();
            if (executionContext is NServiceBusMessageHandlerExecutionContext)
            {
                var nsbStorageSession = sp.GetRequiredService<ISqlStorageSession>();
                return new DbContextOptionsBuilder<WriteModelDbContext>()
                    .UseSqlServer(nsbStorageSession.Connection, sqlServerOptionsAction =>
                        sqlServerOptionsAction
                            .EnableRetryOnFailure()
                            .CommandTimeout(3600)).Options;
            }

            return new DbContextOptionsBuilder<WriteModelDbContext>()
                .UseSqlServer(configuration["ConnectionStrings:Db"], sqlServerOptionsAction =>
                    sqlServerOptionsAction
                        .EnableRetryOnFailure()
                        .CommandTimeout(3600)).Options;
        });

        services.AddScoped<WriteModelDbContext>(sp =>
        {
            var dbContext = new WriteModelDbContext(sp.GetRequiredService<DbContextOptions<WriteModelDbContext>>());
            var executionContext = sp.GetRequiredService<IExecutionContext>();
            if (executionContext is NServiceBusMessageHandlerExecutionContext)
            {
                var nsbStorageSession = sp.GetRequiredService<ISqlStorageSession>();
                dbContext.Database.UseTransaction(nsbStorageSession.Transaction);
            }

            return dbContext;
        });

        services.AddDbContext<ReadModelDbContext>(optionsAction => optionsAction
            .UseSqlServer(configuration["ConnectionStrings:Db"], sqlServerOptionsAction => sqlServerOptionsAction
                .EnableRetryOnFailure()
                .CommandTimeout(3600))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<ITodoReadModelProvider, TodoReadmodelProvider>();
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IPersistenceSession, EfCorePersistenceSession>();

        return services;
    }
}
