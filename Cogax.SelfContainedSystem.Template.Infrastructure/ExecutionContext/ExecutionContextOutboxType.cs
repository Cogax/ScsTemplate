namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public enum ExecutionContextOutboxType
{
    NoOutbox,
    HangfireOutbox,
    NServiceBusOutbox
}
