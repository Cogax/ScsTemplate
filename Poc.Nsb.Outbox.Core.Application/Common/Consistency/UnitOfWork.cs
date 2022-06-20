using Poc.Nsb.Outbox.Core.Application.Common.Ports;

namespace Poc.Nsb.Outbox.Core.Application.Common.Consistency;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly IDomainEventsDispatcher domainEventsDispatcher;
    private readonly IPersistenceAdapter persistenceAdapter;

    public UnitOfWork(
        IDomainEventsDispatcher domainEventsDispatcher,
        IPersistenceAdapter persistenceAdapter)
    {
        this.domainEventsDispatcher = domainEventsDispatcher;
        this.persistenceAdapter = persistenceAdapter;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        // Dispatch all pending Domain Events in memory
        await domainEventsDispatcher.DispatchEventsAsync(cancellationToken);

        return await persistenceAdapter.SaveChangesAsync(cancellationToken);
    }
}
