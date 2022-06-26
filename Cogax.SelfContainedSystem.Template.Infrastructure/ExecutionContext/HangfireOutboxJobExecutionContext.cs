using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class HangfireOutboxJobExecutionContext : IExecutionContext
{
    private readonly IServiceProvider _serviceProvider;

    public HangfireOutboxJobExecutionContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ExecutionContextOutboxType GetExecutionContextOutboxType()
    {
        return ExecutionContextOutboxType.NoOutbox;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return _serviceProvider.GetRequiredService<NullUnitOfWork>();
    }
}
