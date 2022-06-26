using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public interface IExecutionContext
{
    ExecutionContextOutboxType GetExecutionContextOutboxType();
    IUnitOfWork CreateUnitOfWork();
}
