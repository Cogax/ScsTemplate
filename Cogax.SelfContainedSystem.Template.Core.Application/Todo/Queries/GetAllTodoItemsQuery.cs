using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;

public record GetAllTodoItemsQuery : IQuery<IEnumerable<TodoItemDescription>>;

internal class GetAllTodoItemsQueryHandler : IQueryHandler<GetAllTodoItemsQuery, IEnumerable<TodoItemDescription>>
{
    private readonly ITodoReadModelProvider _readModelProvider;

    public GetAllTodoItemsQueryHandler(ITodoReadModelProvider readModelProvider)
    {
        _readModelProvider = readModelProvider;
    }

    public Task<IEnumerable<TodoItemDescription>> Handle(
        GetAllTodoItemsQuery request,
        CancellationToken cancellationToken)
    {
        return _readModelProvider.GetAllTodoItemDescriptions(cancellationToken);
    }
}
