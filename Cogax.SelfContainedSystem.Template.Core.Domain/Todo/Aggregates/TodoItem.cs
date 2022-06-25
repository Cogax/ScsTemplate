using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;

public class TodoItem : AggregateRoot
{
    public TodoItemId Id { get; }
    private Label label;
    private bool completed;
    private bool removed;

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

    public void Remove()
    {
        if (this.removed) return;

        this.removed = true;
        AddDomainEvent(new TodoItemRemovedDomainEvent(Id));
    }

    protected override IEnumerable<object> GetIdentityComponents()
    {
        yield return Id;
    }
}
