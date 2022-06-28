using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IPersistenceSession
{
    /// <summary>
    /// Get all aggregate roots which are "tracked" within this transaction
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A readonly list of aggregate root instances
    /// </returns>
    Task<IReadOnlyCollection<IAggregateRoot>> GetTrackedAggregatesAsync(CancellationToken cancellationToken);
}
