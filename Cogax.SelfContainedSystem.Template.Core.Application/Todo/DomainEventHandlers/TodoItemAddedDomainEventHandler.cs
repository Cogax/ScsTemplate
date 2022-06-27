using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.DomainEventHandlers;

public class TodoItemAddedDomainEventHandler : IDomainEventHandler<TodoItemAddedDomainEvent>
{
    private readonly ITodoMessagingPort _messagingPort;
    private readonly ITodoItemRepository _repository;

    public TodoItemAddedDomainEventHandler(ITodoMessagingPort messagingPort, ITodoItemRepository repository)
    {
        _messagingPort = messagingPort;
        _repository = repository;
    }

    public async Task Handle(TodoItemAddedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _messagingPort.SendIntegrationEvent(domainEvent, cancellationToken);
        await _repository.GetAll(cancellationToken); // Check if Connection is still available after Hangfire Job Enqueue
    }
}
