using Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.Ports;

public interface ITodoItemRepository
{
    Task Add(TodoItem todoItem, CancellationToken cancellationToken = default);
    Task<TodoItem> GetById(TodoItemId id, CancellationToken cancellationToken = default);
    Task ClearAll(CancellationToken cancellationToken = default);
}
