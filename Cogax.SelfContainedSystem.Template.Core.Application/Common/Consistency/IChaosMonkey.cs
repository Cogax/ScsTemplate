namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IChaosMonkey
{
    void AtEndOfUnitOfWorkScope();
    void OnNsbHandleTodoItemAdded();
    void OnNsbHandleTodoItemCompleted();
    void AfterUnitOfWorkCommitted();
}

public class NullChaosMonkey : IChaosMonkey
{
    public void AtEndOfUnitOfWorkScope(){}
    public void OnNsbHandleTodoItemAdded(){}
    public void OnNsbHandleTodoItemCompleted(){}
    public void AfterUnitOfWorkCommitted(){}
}
