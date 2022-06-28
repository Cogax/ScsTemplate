using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

using Hangfire;

using Microsoft.Extensions.Hosting;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire.Recurring;

public class RecurringJobInitializationBackgroundService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        RecurringJob.AddOrUpdate(
            nameof(DeleteRemovedTodoItemsCommand),
            (RecurringJobs jobs) => jobs.DeleteRemovedTodoItems(CancellationToken.None),
            Cron.Never);

        return Task.CompletedTask;
    }
}
