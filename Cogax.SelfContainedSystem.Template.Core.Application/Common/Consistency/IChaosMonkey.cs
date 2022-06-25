namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IChaosMonkey
{
    void OnUowCommit();
    void OnNsbHandle();
}

public class NullChaosMonkey : IChaosMonkey
{
    public void OnUowCommit(){}
    public void OnNsbHandle(){}
}
