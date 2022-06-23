using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

public interface ITodoEmailPort
{
    Task SendEmail(TodoItemId id, CancellationToken cancellationToken);
}
