using NServiceBus;

namespace Poc.Nsb.Outbox.Infrastructure.Events;

public class MyEntityCreatedEvent : IEvent
{
    public Guid Id { get; set; }
}
