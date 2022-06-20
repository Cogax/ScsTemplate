using Dapper;

using Microsoft.EntityFrameworkCore;

using Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;
using Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Common.ValueConverters;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Common;

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
