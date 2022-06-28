namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

public class NsbExecutionContextIdentifierCurrentSessionHolder
{
    public INsbMessageHandlerExecutionContextIdentifier Identifier
    {
        get
        {
            if(_identifier.Value != null)
                return _identifier.Value;
            return new NotInMessageHandlerExecutionContextIdentifier();
        }
    }

    private AsyncLocal<INsbMessageHandlerExecutionContextIdentifier> _identifier = new();

    public NsbExecutionContextIdentifierCurrentSessionHolder()
    {
        _identifier.Value = new NotInMessageHandlerExecutionContextIdentifier();
    }

    public IDisposable OpenInMessageHandlerContextSession()
    {
        _identifier.Value = new InMessageHandlerExecutionContextIdentifier();
        return new NsbExecutionContextIdentifiertMessageHandlerSessionScope(this);
    }

    public void ResetInMessageHandlerContext()
    {
        _identifier.Value = new NotInMessageHandlerExecutionContextIdentifier();
    }
}
