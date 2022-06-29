using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations;

public static class Migrator
{
    public static void Migrate()
    {
        var connectionString =
            new ConfigurationBuilder().ConfigureDefaultConfig("Development", AppContext.BaseDirectory).Build()[
                "ConnectionStrings:Db"];

        var serviveProvider = new ServiceCollection()
            .AddDbContext<WriteModelDbContext>(optionsAction => optionsAction
                .UseSqlServer(connectionString, sqlServerOptionsAction => sqlServerOptionsAction
                    .EnableRetryOnFailure()
                    .CommandTimeout(3600)))
            .BuildServiceProvider();

        using var scope = serviveProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteModelDbContext>();
        dbContext.Database.Migrate();
    }
        
}
