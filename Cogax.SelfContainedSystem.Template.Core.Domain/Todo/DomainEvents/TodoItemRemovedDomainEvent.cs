using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

public class TodoItemRemovedDomainEvent : DomainEvent
{
    public TodoItemId TodoItemId { get; }

    public TodoItemRemovedDomainEvent(TodoItemId id)
    {
        TodoItemId = id;
    }
}
