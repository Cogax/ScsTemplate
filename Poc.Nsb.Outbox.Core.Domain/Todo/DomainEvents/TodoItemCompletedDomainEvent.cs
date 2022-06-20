using Poc.Nsb.Outbox.Core.Domain.Common;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;

public class TodoItemCompletedDomainEvent : DomainEvent
{
    public TodoItemId TodoItemId { get; }

    public TodoItemCompletedDomainEvent(TodoItemId id)
    {
        TodoItemId = id;
    }
}
