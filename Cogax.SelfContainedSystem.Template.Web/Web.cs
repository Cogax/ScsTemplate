using Cogax.SelfContainedSystem.Template.Core.Application;
using Cogax.SelfContainedSystem.Template.Core.Domain;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations;
using Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

using Hangfire;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Web;

public static class Web
{
    public static WebApplication BuildWeb(this WebApplicationBuilder builder)
    {
        //Migrator.Migrate();

        builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
        builder.Host.AddMessaging("Cogax.SelfContainedSystem.Template.Web", enableSendOnly: true, enablePurgeAtStartup: true);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMediatR(typeof(Web));
        builder.Services.AddInfrastructure(builder.Configuration);
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

        return app;
    }
}
