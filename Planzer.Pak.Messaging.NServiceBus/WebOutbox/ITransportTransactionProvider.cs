using NServiceBus.Transport;

namespace Planzer.Pak.Messaging.NServiceBus.WebOutbox;

public interface ITransportTransactionProvider
{
    TransportTransaction TransportTransaction { get; }
}
