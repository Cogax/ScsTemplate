using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

public record DeleteRemovedTodoItemsCommand : ICommand;

public class DeleteRemovedTodoItemsCommandHandler : ICommandHandler<DeleteRemovedTodoItemsCommand>
{
    private readonly ITodoItemRepository _repository;
    private readonly ITodoMessagingPort _messagingPort;

    public DeleteRemovedTodoItemsCommandHandler(ITodoItemRepository repository, ITodoMessagingPort messagingPort)
    {
        _repository = repository;
        _messagingPort = messagingPort;
    }

    public async Task<Unit> Handle(DeleteRemovedTodoItemsCommand request, CancellationToken cancellationToken)
    {
        await _repository.ClearAll(cancellationToken);
        await _messagingPort.SendIntegrationEvent(new TodoItemsDeletedDomainEvent(), cancellationToken);
        return Unit.Value;
    }
}
