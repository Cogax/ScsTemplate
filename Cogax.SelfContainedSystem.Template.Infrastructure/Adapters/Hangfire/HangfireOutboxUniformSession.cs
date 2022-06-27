using Hangfire;

using NServiceBus;
using NServiceBus.UniformSession;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public class HangfireOutboxUniformSession : IUniformSession
{
    public const string JobParameterName = "ProcessHangfireOutbox"; // Changing this may requires DB Migration
    public const string JobParameterValue = "ProcessHangfireOutbox"; // Changing this may requires DB Migration
    private readonly IBackgroundJobClient _hangfireJobClient;

    public HangfireOutboxUniformSession(IBackgroundJobClient hangfireJobClient)
    {
        _hangfireJobClient = hangfireJobClient;
    }

    public Task Send(object message, SendOptions options)
    {
        _hangfireJobClient.Enqueue<HangfireOutboxProcessor>(
            outboxProcessor => outboxProcessor.Send(message, options, null));

        return Task.CompletedTask;
    }

    public Task Send<T>(Action<T> messageConstructor, SendOptions options)
    {
        _hangfireJobClient.Enqueue<HangfireOutboxProcessor>(
            hangfireOutbxProcessor => hangfireOutbxProcessor.Send<T>(messageConstructor, options, null));

        return Task.CompletedTask;
    }

    public Task Publish(object message, PublishOptions options)
    {
        _hangfireJobClient.Enqueue<HangfireOutboxProcessor>(
            hangfireOutbxProcessor => hangfireOutbxProcessor.Publish(message, options, null));

        return Task.CompletedTask;
    }

    public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
    {
        _hangfireJobClient.Enqueue<HangfireOutboxProcessor>(
            hangfireOutbxProcessor => hangfireOutbxProcessor.Publish<T>(messageConstructor, publishOptions, null));

        return Task.CompletedTask;
    }

    public Task Subscribe(Type eventType, SubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");

    public Task Unsubscribe(Type eventType, UnsubscribeOptions options) => throw new NotImplementedException("Hangfire Outbox cannot subscribe Messages!");
}
