using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.DomainEventHandlers;

public class TodoItemRemovdDomainEventHandler : IDomainEventHandler<TodoItemRemovedDomainEvent>
{
    private readonly ITodoMessagingPort _messagingPort;

    public TodoItemRemovdDomainEventHandler(ITodoMessagingPort messagingPort)
    {
        _messagingPort = messagingPort;
    }

    public async Task Handle(TodoItemRemovedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _messagingPort.SendIntegrationEvent(domainEvent, cancellationToken);
    }
}
