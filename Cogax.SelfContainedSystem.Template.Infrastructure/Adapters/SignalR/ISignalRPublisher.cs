using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

public interface ISignalRPublisher
{
    public Task NewTodoItem(TodoItemDescription vm);
    public Task RemoveTodoItemdoItem(Guid id);
}

public class NullSignalRPublisher : ISignalRPublisher
{
    public Task NewTodoItem(TodoItemDescription vm) => Task.CompletedTask;
    public Task RemoveTodoItemdoItem(Guid id) => Task.CompletedTask;
}
