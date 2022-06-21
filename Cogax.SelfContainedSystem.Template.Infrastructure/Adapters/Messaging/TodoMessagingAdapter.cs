using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging;

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
