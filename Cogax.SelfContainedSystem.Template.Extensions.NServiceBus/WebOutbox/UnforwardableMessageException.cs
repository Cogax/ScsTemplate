namespace Cogax.SelfContainedSystem.Template.Extensions.NServiceBus.WebOutbox;

internal class UnforwardableMessageException : Exception
{
    public UnforwardableMessageException(string message) : base(message)
    {
    }
}
