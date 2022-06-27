using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Outbox;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public interface IExecutionContext
{
    IUnitOfWork CreateUnitOfWork();
    IOutbox CreateOutbox();
}
