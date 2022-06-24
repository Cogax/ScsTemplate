using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;

public interface ITodoReadModelProvider
{
    Task<TodoItemDescription> GetTodoItemDescription(TodoItemId id, CancellationToken cancellationToken);
    Task<IEnumerable<TodoItemDescription>> GetAllTodoItemDescriptions(CancellationToken cancellationToken);
}
