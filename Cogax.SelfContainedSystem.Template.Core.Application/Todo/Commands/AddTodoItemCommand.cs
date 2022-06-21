using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

public record AddTodoItemCommand(TodoItemId Id, Label Label) : ICommand;

internal class AddTodoItemCommandHandler : ICommandHandler<AddTodoItemCommand>
{
    private readonly ITodoItemRepository _repository;

    public AddTodoItemCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(AddTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = new TodoItem(command.Id, command.Label);
        await _repository.Add(todoItem, cancellationToken);
        return Unit.Value;
    }
}
