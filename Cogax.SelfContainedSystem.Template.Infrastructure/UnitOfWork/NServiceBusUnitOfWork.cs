using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

// Im Fall von NSB wird bei der WriteModelDbContext registrierung
// der DbContext bereits mit der NSB Storage Session aufgesetzt.
public class NServiceBusUnitOfWork : DefaultUnitOfWork
{
    public NServiceBusUnitOfWork(WriteModelDbContext dbContext) : base(dbContext)
    {
    }
}
