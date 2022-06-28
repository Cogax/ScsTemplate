using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency;

public abstract class BasePersistenceTransaction : IPersistenceTransaction
{
    protected abstract Task<T> Execute<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);

    public async Task<T> ExecuteTransaction<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        try
        {
            return await Execute(operation, cancellationToken);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new AggregateRootConcurrencyException(concurrencyException);
        }
    }
}
