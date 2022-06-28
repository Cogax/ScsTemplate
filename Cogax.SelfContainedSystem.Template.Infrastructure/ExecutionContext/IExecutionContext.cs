using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency.Outbox;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public interface IExecutionContext
{
    IPersistenceTransaction CreatePersistenceTransaction();
    IOutbox CreateOutbox();
}
