namespace Planzer.Pak.Messaging.NServiceBus.WebOutbox;

internal class UnforwardableMessageException : Exception
{
    public UnforwardableMessageException(string message) : base(message)
    {
    }
}
