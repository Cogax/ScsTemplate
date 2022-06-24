using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

public class TodoItemCompletedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
