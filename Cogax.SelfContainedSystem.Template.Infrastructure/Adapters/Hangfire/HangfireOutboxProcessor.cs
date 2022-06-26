using Hangfire.Server;

using NServiceBus;
using NServiceBus.UniformSession;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public class HangfireOutboxProcessor
{
    private readonly IUniformSession _uniformSession;

    public HangfireOutboxProcessor(IUniformSession uniformSession)
    {
        _uniformSession = uniformSession;
    }

    public async Task Send(object message, SendOptions options, PerformContext? performContext)
    {
        performContext?.Items.TryAdd(
            HangfireOutboxUniformSession.JobParameterName,
            HangfireOutboxUniformSession.JobParameterValue);

        await _uniformSession.Send(message, options);
    }

    public async Task Send<T>(Action<T> messageConstructor, SendOptions options, PerformContext? performContext)
    {
        performContext?.Items.TryAdd(
            HangfireOutboxUniformSession.JobParameterName,
            HangfireOutboxUniformSession.JobParameterValue);

        await _uniformSession.Send(messageConstructor, options);
    }

    public async Task Publish(object message, PublishOptions options, PerformContext? performContext)
    {
        performContext?.Items.TryAdd(
            HangfireOutboxUniformSession.JobParameterName,
            HangfireOutboxUniformSession.JobParameterValue);

        await _uniformSession.Publish(message, options);
    }

    public async Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions, PerformContext? performContext)
    {
        performContext?.Items.TryAdd(
            HangfireOutboxUniformSession.JobParameterName,
            HangfireOutboxUniformSession.JobParameterValue);

        await _uniformSession.Publish(messageConstructor, publishOptions);
    }
}
