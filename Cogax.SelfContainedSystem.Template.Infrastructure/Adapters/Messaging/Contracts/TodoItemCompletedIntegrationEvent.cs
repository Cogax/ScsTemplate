using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

public class TodoItemAddedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
