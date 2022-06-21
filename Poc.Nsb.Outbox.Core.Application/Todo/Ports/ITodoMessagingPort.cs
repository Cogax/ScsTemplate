using Poc.Nsb.Outbox.Core.Domain.Todo.DomainEvents;

namespace Poc.Nsb.Outbox.Core.Application.Todo.Ports;

public interface ITodoMessagingPort
{
    Task SendIntegrationEvent(TodoItemAddedDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
