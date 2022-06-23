using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.DomainEventHandlers;

public class TodoItemCompletedDomainEventHandler : IDomainEventHandler<TodoItemCompletedDomainEvent>
{
    private readonly ITodoMessagingPort _messagingPort;

    public TodoItemCompletedDomainEventHandler(ITodoMessagingPort messagingPort)
    {
        _messagingPort = messagingPort;
    }

    public async Task Handle(TodoItemCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _messagingPort.SendIntegrationEvent(domainEvent, cancellationToken);
    }
}
