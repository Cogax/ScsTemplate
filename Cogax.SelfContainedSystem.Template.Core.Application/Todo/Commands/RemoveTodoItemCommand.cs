using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

public record RemoveTodoItemCommand(TodoItemId TodoItemId) : ICommand;

internal class RemoveTodoItemCommandCommandHandler : ICommandHandler<RemoveTodoItemCommand>
{
    private readonly ITodoItemRepository _repository;

    public RemoveTodoItemCommandCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(RemoveTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetById(command.TodoItemId, cancellationToken);
        todoItem.Remove();
        return Unit.Value;
    }
}
