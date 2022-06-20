using Poc.Nsb.Outbox.Core.Application.Common.Exceptions;
using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Application.Common.Ports;

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

    /// <summary>
    /// Save all pending changes to the underlying persistence provider.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Number of affected records</returns>
    /// <exception cref="AggregateRootConcurrencyException">
    /// If a concurrency conflict occures during saving
    /// </exception>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
