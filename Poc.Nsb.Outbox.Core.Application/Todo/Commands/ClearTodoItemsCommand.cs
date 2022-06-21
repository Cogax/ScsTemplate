using MediatR;

using Poc.Nsb.Outbox.Core.Application.Common.Abstractions;
using Poc.Nsb.Outbox.Core.Domain.Todo.Ports;

namespace Poc.Nsb.Outbox.Core.Application.Todo.Commands;

public record ClearTodoItemsCommand() : ICommand;

internal class ClearTodoItemsCommandHandler : ICommandHandler<ClearTodoItemsCommand>
{
    private readonly ITodoItemRepository _repository;

    public ClearTodoItemsCommandHandler(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(ClearTodoItemsCommand request, CancellationToken cancellationToken)
    {
        await _repository.ClearAll(cancellationToken);
        return Unit.Value;
    }
}
