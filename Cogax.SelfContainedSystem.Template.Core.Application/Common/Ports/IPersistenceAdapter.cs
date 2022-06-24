using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;

public interface IPersistenceAdapter
{
    /// <summary>
    /// Get all aggregate roots which are "tracked" within this transaction
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A readonly list of aggregate root instances
    /// </returns>
    Task<IReadOnlyCollection<IAggregateRoot>> GetAllTrackedAggregatesAsync(CancellationToken cancellationToken);

    Task<T> ExecuteWithTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);
}
