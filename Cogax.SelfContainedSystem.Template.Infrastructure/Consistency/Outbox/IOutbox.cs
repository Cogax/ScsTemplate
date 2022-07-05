namespace Cogax.SelfContainedSystem.Template.Infrastructure.Consistency.Outbox;

public interface IOutbox
{
    Task Publish(object message, object options, CancellationToken cancellationToken);
    Task Send(object message, object options, CancellationToken cancellationToken);
}
