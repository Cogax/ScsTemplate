namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

public interface INsbMessageHandlerExecutionContextIdentifier { }
public class InMessageHandlerExecutionContextIdentifier : INsbMessageHandlerExecutionContextIdentifier {}
public class NotInMessageHandlerExecutionContextIdentifier : INsbMessageHandlerExecutionContextIdentifier {}