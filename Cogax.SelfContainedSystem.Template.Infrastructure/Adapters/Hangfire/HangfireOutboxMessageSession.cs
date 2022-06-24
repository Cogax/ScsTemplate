using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

using Hangfire;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public class HangfireOutboxMessageSession : IMessageSession
{
    private readonly IBackgroundJobClient _hangfireJobClient;
    private readonly IChaosMonkey _chaosMonkey;

    public HangfireOutboxMessageSession(IBackgroundJobClient hangfireJobClient, IChaosMonkey chaosMonkey)
    {
        _hangfireJobClient = hangfireJobClient;
        _chaosMonkey = chaosMonkey;
    }

    public Task Send(object message, SendOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Send(message, options));

        _chaosMonkey.OnOutboxAdded();

        return Task.CompletedTask;
    }

    public Task Send<T>(Action<T> messageConstructor, SendOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Send<T>(messageConstructor, options));

        _chaosMonkey.OnOutboxAdded();

        return Task.CompletedTask;
    }

    public Task Publish(object message, PublishOptions options)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Publish(message, options));

        _chaosMonkey.OnOutboxAdded();

        return Task.CompletedTask;
    }

    public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
    {
        _hangfireJobClient.Enqueue<IMessageSession>(
            messageSession => messageSession.Publish<T>(messageConstructor, publishOptions));

        _chaosMonkey.OnOutboxAdded();

        return Task.CompletedTask;
    }

    public Task Subscribe(Type eventType, SubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");

    public Task Unsubscribe(Type eventType, UnsubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");
}
