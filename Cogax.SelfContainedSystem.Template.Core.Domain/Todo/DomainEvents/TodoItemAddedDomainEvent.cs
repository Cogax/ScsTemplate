using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

public class TodoItemAddedDomainEvent : DomainEvent
{
    public TodoItemId TodoItemId { get; }

    public TodoItemAddedDomainEvent(TodoItemId id)
    {
        TodoItemId = id;
    }
}
