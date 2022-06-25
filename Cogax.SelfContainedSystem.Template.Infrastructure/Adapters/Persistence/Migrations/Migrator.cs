using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations;

public static class Migrator
{
    public static void Migrate() =>
        new ServiceCollection()
            .AddPersistenceAdapter(new ConfigurationBuilder()
                .ConfigureDefaultConfig("Development", AppContext.BaseDirectory)
                .Build())
            .BuildServiceProvider()
            .CreateScope().ServiceProvider.GetRequiredService<WriteModelDbContext>()
            .Database.Migrate();
}
