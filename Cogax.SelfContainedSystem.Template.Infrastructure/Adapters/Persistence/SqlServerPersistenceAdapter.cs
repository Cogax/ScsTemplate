using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence;

public class SqlServerPersistenceAdapter : IPersistenceAdapter
{
    private readonly WriteModelDbContext dbContext;

    public SqlServerPersistenceAdapter(WriteModelDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<IReadOnlyCollection<IAggregateRoot>> GetAllTrackedAggregatesAsync(CancellationToken cancellationToken)
    {
        var aggregates = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<IAggregateRoot>>(aggregates);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new AggregateRootConcurrencyException(
                await GetAllTrackedAggregatesAsync(cancellationToken),
                concurrencyException);
        }
    }
}
