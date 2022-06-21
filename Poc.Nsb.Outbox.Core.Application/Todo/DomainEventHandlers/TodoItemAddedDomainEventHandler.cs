using Poc.Nsb.Outbox.Core.Application.Common.Abstractions;
using Poc.Nsb.Outbox.Core.Application.Todo.Ports;
using Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;

namespace Poc.Nsb.Outbox.Core.Application.Todo.DomainEventHandlers;

public class TodoItemAddedDomainEventHandler : IDomainEventHandler<TodoItemAddedDomainEvent>
{
    private readonly ITodoMessagingPort _messagingPort;

    public TodoItemAddedDomainEventHandler(ITodoMessagingPort messagingPort)
    {
        _messagingPort = messagingPort;
    }

    public async Task Handle(TodoItemAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _messagingPort.SendIntegrationEvent(domainEvent, cancellationToken);
    }
}
