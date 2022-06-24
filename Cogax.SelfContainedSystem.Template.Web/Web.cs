using Cogax.SelfContainedSystem.Template.Core.Application;
using Cogax.SelfContainedSystem.Template.Core.Domain;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

using Hangfire;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Web;

public static class Web
{
    public static WebApplication BuildWeb(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
        builder.Host.AddMessaging("Cogax.SelfContainedSystem.Template.Web", enableSendOnly: true, enableWebOutbox: false, enableNsbOutbox: true);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMediatR(typeof(Web));
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHangfireOutboxAdapter(builder.Configuration);
        builder.Services.AddCoreApplication();
        builder.Services.AddCoreDomain();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();
        app.UseHangfireDashboard();
        app.MapHangfireDashboard("/jobs", new DashboardOptions
        {
            IgnoreAntiforgeryToken = true,
            Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
        });
        app.Services.Migrate<WriteModelDbContext>();

        return app;
    }
}
