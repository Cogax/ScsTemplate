using NServiceBus;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Messaging.Contracts;

public class TodoItemAddedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
