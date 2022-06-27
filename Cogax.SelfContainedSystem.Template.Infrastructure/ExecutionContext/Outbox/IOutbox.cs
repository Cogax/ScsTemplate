namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Outbox;

public interface IOutbox
{
    Task Publish(object message, object options, CancellationToken cancellationToken);
    Task Send(object message, object options, CancellationToken cancellationToken);
}
