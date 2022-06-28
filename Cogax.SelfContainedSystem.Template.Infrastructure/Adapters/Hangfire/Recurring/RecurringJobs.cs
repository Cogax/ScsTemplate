using System.ComponentModel;

using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

using Hangfire;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire.Recurring;

public class RecurringJobs
{
    private readonly IMediator _mediator;

    public RecurringJobs(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName($"DeleteRemovedTodoItems")]
    [DisableConcurrentExecution(5)]
    [AutomaticRetry(Attempts = 0, LogEvents = false, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task DeleteRemovedTodoItems(CancellationToken cancellationToken)
        => await _mediator.Send(new DeleteRemovedTodoItemsCommand(), cancellationToken);
}
