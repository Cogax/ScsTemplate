namespace Poc.Nsb.Outbox.Core.Domain.Common;

public interface IBusinessRule
{
    bool IsBroken();

    BusinessRuleMessage Message { get; }
}
