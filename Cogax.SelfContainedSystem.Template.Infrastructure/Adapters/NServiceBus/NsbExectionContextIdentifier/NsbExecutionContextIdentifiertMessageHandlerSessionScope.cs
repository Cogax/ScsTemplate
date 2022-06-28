namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

public class NsbExecutionContextIdentifiertMessageHandlerSessionScope : IDisposable
{
    private readonly NsbExecutionContextIdentifierCurrentSessionHolder _sessionHolder;

    public NsbExecutionContextIdentifiertMessageHandlerSessionScope(NsbExecutionContextIdentifierCurrentSessionHolder sessionHolder)
    {
        _sessionHolder = sessionHolder;
    }

    public void Dispose()
    {
        _sessionHolder.ResetInMessageHandlerContext();
    }
}
