using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

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
