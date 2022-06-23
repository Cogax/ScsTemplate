using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Email;

public class NullTodoEmailAdapter : ITodoEmailPort
{
    public List<object> Emails = new();

    public Task SendEmail(TodoItemId id, CancellationToken cancellationToken)
    {
        Emails.Add(id);
        return Task.CompletedTask;
    }
}
