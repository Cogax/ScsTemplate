using Cogax.SelfContainedSystem.Template.Core.Application;
using Cogax.SelfContainedSystem.Template.Core.Domain;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Extensions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Worker;

public static class Worker
{
    public static WebApplication BuildWorker(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
        builder.Host.AddMessaging("Cogax.SelfContainedSystem.Template.Worker", enableSendOnly: false, enablePurgeAtStartup: true);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddMediatR(typeof(Worker));
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddHangfireServerAdapter(builder.Configuration);
        builder.Services.AddCoreApplication();
        builder.Services.AddCoreDomain();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
