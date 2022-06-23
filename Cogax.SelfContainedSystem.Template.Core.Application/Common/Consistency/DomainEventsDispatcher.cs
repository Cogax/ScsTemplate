using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IDomainEventsDispatcher
{
    Task DispatchEventsAsync(CancellationToken cancellationToken);
}

internal class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IPersistenceAdapter persistenceAdapter;
    private readonly IMediator mediator;

    public DomainEventsDispatcher(
        IPersistenceAdapter persistenceAdapter,
        IMediator mediator)
    {
        this.persistenceAdapter = persistenceAdapter;
        this.mediator = mediator;
    }

    public async Task DispatchEventsAsync(CancellationToken cancellationToken)
    {
        // Dispatch all domain events as long as there aren't any on any tracked entity/aggregate.
        // This ensures consitency because domain events are published within the same transaction.

        List<bool> areEventsPublishedPerAggregate;
        do
        {
            var aggregates = await persistenceAdapter.GetAllTrackedAggregatesAsync(cancellationToken);
            if (!aggregates.Any())
                break;

            areEventsPublishedPerAggregate = new List<bool>();
            foreach (var aggregate in aggregates)
            {
                areEventsPublishedPerAggregate.Add(await PublishEvents(aggregate, cancellationToken));
            }
        } while (areEventsPublishedPerAggregate.Any(areEventsPublised => areEventsPublised));
    }

    private async Task<bool> PublishEvents(IAggregateRoot aggregate, CancellationToken cancellationToken)
    {
        var domainEvents = DomainEventsHelper.RetrieveAggregatedDomainEvents(aggregate);

        // If an Aggregate (or it's containing entities) doesn't have any domain events,
        // we assume there has nothing changed. Therefore we don't need to increment the version.
        if (!domainEvents.Any())
            return false;

        aggregate.IncrementVersion();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }

        return true;
    }
}
