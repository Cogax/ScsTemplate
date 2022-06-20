using System.Security.Principal;

namespace Poc.Nsb.Outbox.Core.Domain.Common;

public interface IAggregateRoot : IEntity
{
    void IncrementVersion();
}

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private int version;

    public virtual void IncrementVersion()
    {
        version++;
    }
}
