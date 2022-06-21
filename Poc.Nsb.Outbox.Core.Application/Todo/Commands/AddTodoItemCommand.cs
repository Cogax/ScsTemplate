using MediatR;

using Poc.Nsb.Outbox.Core.Application.Common.Abstractions;
using Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;
using Poc.Nsb.Outbox.Core.Domain.Todo.Ports;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Application.Todo.Commands;

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
