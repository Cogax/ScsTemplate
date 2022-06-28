using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

    public UnitOfWorkBehavior(
        IUnitOfWork unitOfWork,
        ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is ICommand ||
            (request.GetType().IsGenericType && request.GetType().GetGenericTypeDefinition() == typeof(ICommand<>)))
        {
            _logger.LogDebug($"CommandUnitOfWorkBehavior: Before handle request. UnitOfWork: {_unitOfWork.GetHashCode()}");
            var response = await _unitOfWork.ExecuteBusinessOperation(async (_) => await next(), cancellationToken);
            _logger.LogDebug($"CommandUnitOfWorkBehavior: After handle request. UnitOfWork: {_unitOfWork.GetHashCode()}");

            return response;
        }

        return await next();
    }
}
