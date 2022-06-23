using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ReadmodelProviders;

public class TodoReadmodelProvider : ITodoReadModelProvider
{
    private readonly ReadModelDbContext _readModelDb;

    public TodoReadmodelProvider(ReadModelDbContext readModelDb)
    {
        _readModelDb = readModelDb;
    }

    public async Task<TodoItemDescription> GetTodoItemDescription(TodoItemId id, CancellationToken cancellationToken)
    {
        var todoItem = await _readModelDb.TodoItems.SingleAsync(i => i.Id == id.Value, cancellationToken);
        return new TodoItemDescription(todoItem.Id, todoItem.Label, todoItem.Completed);
    }
}
