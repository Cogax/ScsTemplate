using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;

public interface ICommand : IRequest { }
public interface ICommandHandler<in T> : IRequestHandler<T>
    where T : ICommand
{ }

public interface ICommand<out T> : IRequest<T> { }
public interface ICommandHandler<in T, TOut> : IRequestHandler<T, TOut>
    where T : ICommand<TOut>
{ }

