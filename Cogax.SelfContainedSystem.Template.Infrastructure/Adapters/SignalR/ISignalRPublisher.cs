using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;

namespace Cogax.SelfContainedSystem.Template.Web.SignalR;

public interface ISignalRPublisher
{
    public Task PublishTodoItem(TodoItemDescription vm);
}

public class NullSignalRPublisher : ISignalRPublisher
{
    public List<object> PublishedMessages { get; } = new();

    public Task PublishTodoItem(TodoItemDescription vm)
    {
        PublishedMessages.Add(vm);
        return Task.CompletedTask;
    }
}
