using Hangfire;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency.Outbox;

/// <summary>
/// Publish Messages via Hangfire Outbox.
/// </summary>
public class HangfireOutbox : IOutbox
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireOutbox(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public Task Publish(object message, object options, CancellationToken cancellationToken)
    {
        _backgroundJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Publish(message, options as PublishOptions));

        return Task.CompletedTask;
    }

    public Task Send(object message, object options, CancellationToken cancellationToken)
    {
        _backgroundJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Send(message, options as SendOptions));

        return Task.CompletedTask;
    }
}
