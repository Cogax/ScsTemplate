using Poc.Nsb.Outbox.Core.Domain.Common;
using Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;

public class TodoItem : AggregateRoot
{
    public TodoItemId Id { get; }
    private Label label;
    private bool completed;

    public TodoItem(TodoItemId id, Label label)
    {
        Id = id;
        this.label = label;
        completed = false;
        
        AddDomainEvent(new TodoItemAddedDomainEvent(Id));
    }

    private TodoItem()
    {
        // EF Core
        Id = null!;
        label = null!;
    }

    public void Complete()
    {
        if (this.completed) return;

        this.completed = true;
        AddDomainEvent(new TodoItemCompletedDomainEvent(Id));
    }

    protected override IEnumerable<object> GetIdentityComponents()
    {
        yield return Id;
    }
}
