using Microsoft.EntityFrameworkCore;

using Poc.Nsb.Outbox.Core.Application.Common.Exceptions;
using Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;
using Poc.Nsb.Outbox.Core.Domain.Todo.Ports;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;
using Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Contexts;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Repositories;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly WriteModelDbContext _writeDb;

    public TodoItemRepository(WriteModelDbContext writeDb)
    {
        _writeDb = writeDb;
    }

    public async Task Add(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        await _writeDb.TodoItems.AddAsync(todoItem, cancellationToken);
    }

    public async Task<TodoItem> GetById(TodoItemId id, CancellationToken cancellationToken = default)
    {
        var todoItem = await _writeDb.TodoItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (todoItem is null)
            throw new AggregateRootNotFoundException(nameof(TodoItem),
                new KeyValuePair<string, object>(nameof(TodoItem.Id), id));

        return todoItem;
    }

    public async Task ClearAll(CancellationToken cancellationToken = default)
    {
        _writeDb.TodoItems.RemoveRange(await _writeDb.TodoItems.ToListAsync(cancellationToken));
    }
}
