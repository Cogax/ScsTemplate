using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

public interface ISignalRPublisher
{
    public Task PublishTodoItem(TodoItemDescription? vm);
}

public class NullSignalRPublisher : ISignalRPublisher
{
    public Task PublishTodoItem(TodoItemDescription? vm) => Task.CompletedTask;
}
