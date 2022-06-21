using NServiceBus.Transport;

namespace Cogax.SelfContainedSystem.Template.Extensions.NServiceBus.WebOutbox;

public interface ITransportTransactionProvider
{
    TransportTransaction TransportTransaction { get; }
}
