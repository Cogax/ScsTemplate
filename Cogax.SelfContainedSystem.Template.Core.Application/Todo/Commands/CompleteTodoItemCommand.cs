using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

public record CompleteTodoItemCommand(TodoItemId Id) : ICommand;

internal class CompleteTodoItemCommandHandler : ICommandHandler<CompleteTodoItemCommand>
{
    private readonly ITodoItemRepository _repository;

    public CompleteTodoItemCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(CompleteTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await _repository.GetById(command.Id, cancellationToken);
        todoItem.Complete();
        return Unit.Value;
    }
}
