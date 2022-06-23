using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly IDomainEventsDispatcher domainEventsDispatcher;
    private readonly IPersistenceAdapter persistenceAdapter;
    private readonly IChaosMonkey _chaosMonkey;

    public UnitOfWork(
        IDomainEventsDispatcher domainEventsDispatcher,
        IPersistenceAdapter persistenceAdapter,
        IChaosMonkey chaosMonkey)
    {
        this.domainEventsDispatcher = domainEventsDispatcher;
        this.persistenceAdapter = persistenceAdapter;
        _chaosMonkey = chaosMonkey;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        // Dispatch all pending Domain Events in memory
        await domainEventsDispatcher.DispatchEventsAsync(cancellationToken);
        _chaosMonkey.OnUowCommit();
        return await persistenceAdapter.SaveChangesAsync(cancellationToken);
    }
}
