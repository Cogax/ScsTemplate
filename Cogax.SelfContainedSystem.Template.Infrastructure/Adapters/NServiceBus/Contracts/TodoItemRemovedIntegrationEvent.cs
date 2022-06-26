using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;

public class TodoItemRemovedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
