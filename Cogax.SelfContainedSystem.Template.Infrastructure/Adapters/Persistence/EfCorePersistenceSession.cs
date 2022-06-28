using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence;

public class EfCorePersistenceSession : IPersistenceSession
{
    private readonly WriteModelDbContext dbContext;

    public EfCorePersistenceSession(WriteModelDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<IReadOnlyCollection<IAggregateRoot>> GetTrackedAggregatesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(dbContext.GetTracked<IAggregateRoot>());
    }
}
