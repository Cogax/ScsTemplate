namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IChaosMonkey
{
    void OnUowCommit();
    void OnWebNsbHandle();
    void OnWorkerNsbHandle();
}

public class NullChaosMonkey : IChaosMonkey
{
    public void OnUowCommit(){}
    public void OnWebNsbHandle(){}
    public void OnWorkerNsbHandle(){}
}
