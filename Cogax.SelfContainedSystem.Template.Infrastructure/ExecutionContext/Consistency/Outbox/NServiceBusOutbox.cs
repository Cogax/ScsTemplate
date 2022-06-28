using NServiceBus;
using NServiceBus.UniformSession;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency.Outbox;

/// <summary>
/// Publish Messages via NServiceBis Outbox.
/// <see cref="IUniformSession"/> abstracts <see cref="IMessageSession"/> and <see cref="IMessageHandlerContext"/>.
/// Outbox is only enabled within a message handler.
/// </summary>
public class NServiceBusOutbox : IOutbox
{
    private readonly IUniformSession _uniformSession;

    public NServiceBusOutbox(IUniformSession uniformSession)
    {
        _uniformSession = uniformSession;
    }

    public async Task Publish(object message, object options, CancellationToken cancellationToken)
    {
        await _uniformSession.Publish(message, options as PublishOptions);
    }

    public async Task Send(object message, object options, CancellationToken cancellationToken)
    {
        await _uniformSession.Send(message, options as SendOptions);
    }
}
