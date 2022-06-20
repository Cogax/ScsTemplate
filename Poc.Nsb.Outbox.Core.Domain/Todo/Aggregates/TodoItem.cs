using Poc.Nsb.Outbox.Core.Domain.Common;
using Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;

public class TodoItem : AggregateRoot
{
    private readonly TodoItemId id;
    private Label label;
    private bool completed;

    public TodoItem(TodoItemId id, Label label)
    {
        this.id = id;
        this.label = label;
        completed = false;
    }

    private TodoItem()
    {
        // EF Core
        id = null!;
        label = null!;
    }

    public void Complete()
    {
        if (this.completed) return;

        this.completed = true;
        AddDomainEvent(new TodoItemCompletedDomainEvent(id));
    }

    protected override IEnumerable<object> GetIdentityComponents()
    {
        yield return id;
    }
}
