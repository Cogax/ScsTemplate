using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class DefaultExecutionContext : IExecutionContext
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultExecutionContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ExecutionContextOutboxType GetExecutionContextOutboxType() =>
        ExecutionContextOutboxType.HangfireOutbox;

    public IUnitOfWork CreateUnitOfWork() =>
        _serviceProvider.GetRequiredService<DefaultUnitOfWork>();
}
