using Poc.Nsb.Outbox.Core.Domain.Common;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;

public class TodoItemAddedDomainEvent : DomainEvent
{
    public TodoItemId TodoItemId { get; }

    public TodoItemAddedDomainEvent(TodoItemId id)
    {
        TodoItemId = id;
    }
}
