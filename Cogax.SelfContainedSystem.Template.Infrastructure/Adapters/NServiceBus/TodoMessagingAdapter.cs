using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;

using NServiceBus;
using NServiceBus.UniformSession;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus;

public class TodoMessagingAdapter : ITodoMessagingPort
{
    private readonly IUniformSession _uniformSession;

    public TodoMessagingAdapter(IUniformSession uniformSession)
    {
        _uniformSession = uniformSession;
    }

    public async Task SendIntegrationEvent(
        TodoItemAddedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _uniformSession.Publish(new TodoItemAddedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }

    public async Task SendIntegrationEvent(
        TodoItemCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _uniformSession.Publish(new TodoItemCompletedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }

    public async Task SendIntegrationEvent(
        TodoItemRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await _uniformSession.Publish(new TodoItemRemovedIntegrationEvent
        {
            TodoItemId = domainEvent.TodoItemId.Value
        });
    }
}
