using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ValueConverters;

using Dapper;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Contexts;

public class WriteModelDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public WriteModelDbContext(DbContextOptions<WriteModelDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceAdapterServiceCollectionExtensions).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<TodoItemId>().HaveConversion<TodoItemIdToGuidConverter>();
        SqlMapper.AddTypeHandler<TodoItemId>(new SortieranlageIdToLongTypeHandler());

        configurationBuilder.Properties<Label>().HaveConversion<LabelToStringConverter>();
        SqlMapper.AddTypeHandler<Label>(new LabelToStringTypeHandler());
    }
}
