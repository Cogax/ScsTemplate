using Hangfire.Dashboard;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
