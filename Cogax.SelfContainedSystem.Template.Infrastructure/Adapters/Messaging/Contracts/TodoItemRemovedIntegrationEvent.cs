using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

public class TodoItemRemovedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
