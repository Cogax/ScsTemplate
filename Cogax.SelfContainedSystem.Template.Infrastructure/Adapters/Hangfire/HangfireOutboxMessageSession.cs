using Hangfire;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public class HangfireOutboxMessageSession : IMessageSession
{
    private readonly IBackgroundJobClient _hangfireJobClient;

    public HangfireOutboxMessageSession(IBackgroundJobClient hangfireJobClient)
    {
        _hangfireJobClient = hangfireJobClient;
    }

    public Task Send(object message, SendOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Send(message, options));

        return Task.CompletedTask;
    }

    public Task Send<T>(Action<T> messageConstructor, SendOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Send<T>(messageConstructor, options));

        return Task.CompletedTask;
    }

    public Task Publish(object message, PublishOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Publish(message, options));

        return Task.CompletedTask;
    }

    public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Publish<T>(messageConstructor, publishOptions));

        return Task.CompletedTask;
    }

    public Task Subscribe(Type eventType, SubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");

    public Task Unsubscribe(Type eventType, UnsubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");
}
