namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IChaosMonkey
{
    void OnUowCommit();
    void OnWebNsbHandle();
    void OnWorkerNsbHandle();
}
