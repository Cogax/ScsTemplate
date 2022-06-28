using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;

public static class DbContextExtensions
{
    public static IReadOnlyCollection<TEntity> GetTracked<TEntity>(this DbContext dbContext)
        where TEntity : class
    {
        var entities = dbContext.ChangeTracker
            .Entries<TEntity>()
            .Select(x => x.Entity)
            .ToList();

        return entities;
    }
}
