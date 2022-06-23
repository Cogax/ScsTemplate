using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;

public record GetTodoItemDescriptionQuery(TodoItemId Id) : IQuery<TodoItemDescription>;

internal class GetTodoItemDescriptionQueryHandler : IQueryHandler<GetTodoItemDescriptionQuery, TodoItemDescription>
{
    private readonly ITodoReadModelProvider _readModelProvider;

    public GetTodoItemDescriptionQueryHandler(ITodoReadModelProvider readModelProvider)
    {
        _readModelProvider = readModelProvider;
    }

    public async Task<TodoItemDescription> Handle(GetTodoItemDescriptionQuery request, CancellationToken cancellationToken)
    {
        return await _readModelProvider.GetTodoItemDescription(request.Id, cancellationToken);
    }
}
