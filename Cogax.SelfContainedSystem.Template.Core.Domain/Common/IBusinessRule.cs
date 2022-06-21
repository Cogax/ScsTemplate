namespace Cogax.SelfContainedSystem.Template.Core.Domain.Common;

public interface IBusinessRule
{
    bool IsBroken();

    BusinessRuleMessage Message { get; }
}
