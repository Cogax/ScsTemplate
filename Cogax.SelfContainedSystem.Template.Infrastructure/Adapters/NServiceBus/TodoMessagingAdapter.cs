using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Consistency.Outbox;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus;

public class TodoMessagingAdapter : ITodoMessagingPort
{
    private readonly IOutbox _outbox;

    public TodoMessagingAdapter(IOutbox outbox)
    {
        _outbox = outbox;
    }

    public async Task SendIntegrationEvent(
        TodoItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _outbox.Publish(new TodoItemAddedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }

    public async Task SendIntegrationEvent(
        TodoItemCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _outbox.Publish(new TodoItemCompletedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }

    public async Task SendIntegrationEvent(
        TodoItemRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _outbox.Publish(new TodoItemRemovedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }

    public async Task SendIntegrationEvent(
        TodoItemsDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _outbox.Publish(new TodoItemsDeletedIntegrationEvent());
    }
}
