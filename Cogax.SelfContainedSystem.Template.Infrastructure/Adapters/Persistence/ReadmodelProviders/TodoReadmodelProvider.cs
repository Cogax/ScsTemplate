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

    public async Task<TodoItemDescription?> GetTodoItemDescription(TodoItemId id, CancellationToken cancellationToken)
    {
        var todoItem = await _readModelDb.TodoItems.FirstOrDefaultAsync(i => i.Id == id.Value, cancellationToken);
        return todoItem == null ? null : new TodoItemDescription(todoItem.Id, todoItem.Label, todoItem.Completed);
    }

    public async Task<IEnumerable<TodoItemDescription>> GetAllTodoItemDescriptions(CancellationToken cancellationToken)
    {
        return await _readModelDb.TodoItems
            .Select(i => new TodoItemDescription(i.Id, i.Label, i.Completed))
            .ToListAsync(cancellationToken);
    }
}
