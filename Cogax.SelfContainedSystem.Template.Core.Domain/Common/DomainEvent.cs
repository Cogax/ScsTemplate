using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Common;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public Guid Id { get; }
    public DateTimeOffset OccurredOn { get; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTimeOffset.Now;
    }
}
