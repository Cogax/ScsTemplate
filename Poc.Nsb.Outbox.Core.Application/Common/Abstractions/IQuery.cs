using MediatR;

namespace Poc.Nsb.Outbox.Core.Application.Common.Abstractions;

public interface IQuery<out T> : IRequest<T> { }
public interface IQueryHandler<in T, TOut> : IRequestHandler<T, TOut>
    where T : IQuery<TOut>
{ }
