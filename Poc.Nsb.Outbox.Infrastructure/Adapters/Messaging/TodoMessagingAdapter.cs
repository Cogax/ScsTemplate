using NServiceBus;

using Poc.Nsb.Outbox.Core.Application.Todo.Ports;
using Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;
using Poc.Nsb.Outbox.Infrastructure.Adapters.Messaging.Contracts;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Messaging;

public class TodoMessagingAdapter : ITodoMessagingPort
{
    private readonly IMessageSession _messageSession;

    public TodoMessagingAdapter(IMessageSession messageSession)
    {
        _messageSession = messageSession;
    }

    public async Task SendIntegrationEvent(
        TodoItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _messageSession.Publish(new TodoItemAddedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }
}
