using NServiceBus.Pipeline;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

public class NsbExecutionContextIdentifierBehavior : IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext>
{
    private readonly NsbExecutionContextIdentifierCurrentSessionHolder _sessionHolder;

    public NsbExecutionContextIdentifierBehavior(NsbExecutionContextIdentifierCurrentSessionHolder sessionHolder)
    {
        _sessionHolder = sessionHolder;
    }

    public async Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
    {
        using (_sessionHolder.OpenInMessageHandlerContextSession())
        {
            await next(context).ConfigureAwait(false);
        }
    }
}
