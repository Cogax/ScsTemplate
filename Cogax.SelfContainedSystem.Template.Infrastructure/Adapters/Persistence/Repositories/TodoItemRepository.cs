using System.Data;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Repositories;

public class TodoItemRepository : ITodoItemRepository
{
    private readonly WriteModelDbContext _writeDb;
    private readonly IExecutionContext _executionContext;

    public TodoItemRepository(WriteModelDbContext writeDb, IExecutionContext executionContext)
    {
        _writeDb = writeDb;
        _executionContext = executionContext;
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
        var x = _executionContext;
        _writeDb.TodoItems.RemoveRange(await _writeDb.TodoItems.ToListAsync(cancellationToken));
    }

    public async Task VerifyConnectionOpen(CancellationToken cancellationToken = default)
    {
        var canConnect = await _writeDb.Database.CanConnectAsync(cancellationToken);
        var connection = _writeDb.Database.GetDbConnection();

        if (!canConnect || connection.State != ConnectionState.Open)
            throw new Exception("Connection Closed!");
    }
}
