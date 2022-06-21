using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.DomainEvents;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

public interface ITodoMessagingPort
{
    Task SendIntegrationEvent(TodoItemAddedDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
