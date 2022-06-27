using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Outbox;

public static class OutboxExtensions
{
    public static Task Send(this IOutbox outbox, object message, CancellationToken cancellationToken = default)
    {
        return outbox.Send(message, new SendOptions(), cancellationToken);
    }

    public static Task Send<T>(this IOutbox outbox, Action<T> messageConstructor, CancellationToken cancellationToken = default)
    {
        return outbox.Send(messageConstructor, new SendOptions(), cancellationToken);
    }

    public static Task Send(this IOutbox outbox, string destination, object message, CancellationToken cancellationToken = default)
    {
        var options = new SendOptions();
        options.SetDestination(destination);
        return outbox.Send(message, options, cancellationToken);
    }

    public static Task Send<T>(this IOutbox outbox, string destination, Action<T> messageConstructor, CancellationToken cancellationToken = default)
    {
        var options = new SendOptions();
        options.SetDestination(destination);
        return outbox.Send(messageConstructor, options, cancellationToken);
    }

    public static Task SendLocal(this IOutbox outbox, object message, CancellationToken cancellationToken = default)
    {
        var options = new SendOptions();
        options.RouteToThisEndpoint();
        return outbox.Send(message, options, cancellationToken);
    }

    public static Task SendLocal<T>(this IOutbox outbox, Action<T> messageConstructor, CancellationToken cancellationToken = default)
    {
        var options = new SendOptions();
        options.RouteToThisEndpoint();
        return outbox.Send(messageConstructor, options, cancellationToken);
    }

    public static Task Publish(this IOutbox outbox, object message, CancellationToken cancellationToken = default)
    {
        return outbox.Publish(message, new PublishOptions(), cancellationToken);
    }
    
    public static Task Publish<T>(this IOutbox outbox, CancellationToken cancellationToken = default)
    {
        return outbox.Publish<T>(_ => { }, cancellationToken);
    }

    public static Task Publish<T>(this IOutbox outbox, Action<T> messageConstructor, CancellationToken cancellationToken = default)
    {
        return outbox.Publish(messageConstructor, new PublishOptions(), cancellationToken);
    }
}
