using MediatR;

using Microsoft.Extensions.Logging;

using Poc.Nsb.Outbox.Core.Application.Common.Abstractions;

using ICommand = System.Windows.Input.ICommand;

namespace Poc.Nsb.Outbox.Core.Application.Common.Consistency;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is ICommand ||
            (request.GetType().IsGenericType && request.GetType().GetGenericTypeDefinition() == typeof(ICommand<>)))
        {
            logger.LogDebug($"CommandUnitOfWorkBehavior: Before handle request. UnitOfWork: {unitOfWork.GetHashCode()}");
            var response = await next();
            await unitOfWork.CommitAsync(cancellationToken);
            logger.LogDebug($"CommandUnitOfWorkBehavior: After handle request. UnitOfWork: {unitOfWork.GetHashCode()}");

            return response;
        }

        return await next();
    }
}
