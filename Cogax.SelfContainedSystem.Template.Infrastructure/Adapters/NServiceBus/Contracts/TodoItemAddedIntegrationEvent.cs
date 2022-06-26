using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;

public class TodoItemAddedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
