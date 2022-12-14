using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

public interface ITodoMessagingPort
{
    Task SendIntegrationEvent(TodoItemAddedDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task SendIntegrationEvent(TodoItemCompletedDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task SendIntegrationEvent(TodoItemRemovedDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task SendIntegrationEvent(TodoItemsDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
