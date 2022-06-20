using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Application.Common.Exceptions;

public class AggregateRootConcurrencyException : Exception
{
    public IReadOnlyCollection<IAggregateRoot> AggregateRoots;

    public AggregateRootConcurrencyException(IReadOnlyCollection<IAggregateRoot> aggregateRoots, Exception innerException)
        : base($"Concurrency Exception occured", innerException)
    {
        AggregateRoots = aggregateRoots;
    }
}
