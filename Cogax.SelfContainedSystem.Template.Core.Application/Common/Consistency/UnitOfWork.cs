using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IUnitOfWork
{
    Task<T> ExecuteOperation<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);
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

    public async Task<T> ExecuteOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        return await persistenceAdapter.ExecuteWithTransactionAsync(
            operation: async (cToken) =>
            {
                var result = await operation(cToken);
                await domainEventsDispatcher.DispatchEventsAsync(cancellationToken);
                _chaosMonkey.OnUowCommit();
                return result;
            },
            cancellationToken: cancellationToken);
    }
}
