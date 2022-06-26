namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
