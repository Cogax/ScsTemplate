using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;

public interface ITodoItemRepository
{
    Task Add(TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<TodoItem> GetById(TodoItemId id, CancellationToken cancellationToken = default);
    Task ClearAll(CancellationToken cancellationToken = default);

    // Only for testing purposes
    Task VerifyConnectionOpen(CancellationToken cancellationToken = default);
}
