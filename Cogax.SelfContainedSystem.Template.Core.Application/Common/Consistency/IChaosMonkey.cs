namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IChaosMonkey
{
    void OnUowCommit();
    void OnNsbHandleTodoItemAdded();
    void OnNsbHandleTodoItemCompleted();
}

public class NullChaosMonkey : IChaosMonkey
{
    public void OnUowCommit(){}
    public void OnNsbHandleTodoItemAdded(){}
    public void OnNsbHandleTodoItemCompleted(){}
}
