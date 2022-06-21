using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;

public interface IDomainEventHandler<in T> : INotificationHandler<T> where T : IDomainEvent, INotification
{
}
