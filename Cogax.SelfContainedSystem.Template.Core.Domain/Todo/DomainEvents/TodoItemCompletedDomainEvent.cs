using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

public class TodoItemCompletedDomainEvent : DomainEvent
{
    public TodoItemId TodoItemId { get; }

    public TodoItemCompletedDomainEvent(TodoItemId id)
    {
        TodoItemId = id;
    }
}
