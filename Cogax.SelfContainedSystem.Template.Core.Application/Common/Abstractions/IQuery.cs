using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;

public interface IQuery<out T> : IRequest<T> { }
public interface IQueryHandler<in T, TOut> : IRequestHandler<T, TOut>
    where T : IQuery<TOut>
{ }
