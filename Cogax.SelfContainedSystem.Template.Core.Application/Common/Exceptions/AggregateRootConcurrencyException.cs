using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;

public class AggregateRootConcurrencyException : Exception
{
    public IReadOnlyCollection<IAggregateRoot> AggregateRoots;

    public AggregateRootConcurrencyException(IReadOnlyCollection<IAggregateRoot> aggregateRoots, Exception innerException)
        : base($"Concurrency Exception occured", innerException)
    {
        AggregateRoots = aggregateRoots;
    }
}
