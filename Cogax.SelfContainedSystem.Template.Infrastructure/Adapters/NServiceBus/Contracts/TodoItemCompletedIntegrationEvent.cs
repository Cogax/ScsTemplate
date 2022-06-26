using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;

public class TodoItemCompletedIntegrationEvent : IEvent
{
    public Guid TodoItemId { get; set; }
}
