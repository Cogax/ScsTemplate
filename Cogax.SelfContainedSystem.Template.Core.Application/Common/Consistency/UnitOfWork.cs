namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public class UnitOfWork : IUnitOfWork
{
    private readonly IPersistenceTransaction _persistenceTransaction;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;
    private readonly IChaosMonkey _chaosMonkey;

    public UnitOfWork(
        IPersistenceTransaction persistenceTransaction,
        IDomainEventsDispatcher domainEventsDispatcher,
        IChaosMonkey chaosMonkey)
    {
        _persistenceTransaction = persistenceTransaction;
        _domainEventsDispatcher = domainEventsDispatcher;
        _chaosMonkey = chaosMonkey;
    }

    public async Task<T> ExecuteBusinessOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        return await _persistenceTransaction.ExecuteTransaction(operation: async (cToken) =>
        {
            var result = await operation(cToken);
            await _domainEventsDispatcher.DispatchEventsAsync(cToken);

            _chaosMonkey.OnUowCommit();

            return result;
        }, cancellationToken);
    }
}
