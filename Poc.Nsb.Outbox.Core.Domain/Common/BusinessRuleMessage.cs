namespace Poc.Nsb.Outbox.Core.Domain.Common;

public class BusinessRuleMessage
{
    public string TranslationKey { get; }
    public IEnumerable<object> Arguments { get; }

    public BusinessRuleMessage(string translationKey) : this(translationKey, Enumerable.Empty<object>())
    {
    }

    public BusinessRuleMessage(string translationKey, params object[] args)
    {
        TranslationKey = translationKey;
        Arguments = args;
    }
}
