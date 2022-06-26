using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IExecutionContext _executionContext;

    public UnitOfWorkFactory(IExecutionContext executionContext)
    {
        _executionContext = executionContext;
    }

    public IUnitOfWork Create() =>
        _executionContext.CreateUnitOfWork();
}
