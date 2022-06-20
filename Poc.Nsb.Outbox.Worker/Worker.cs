using Poc.Nsb.Outbox.Infrastructure.Extensions;

namespace Poc.Nsb.Outbox.Worker;

public static class Worker
{
    public static WebApplication BuildWorker(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
        builder.Host.AddMessaging("Poc.Nsb.Outbox.Worker", enableSendOnly: false, enableWebOutbox: false, enableNsbOutbox: true);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSingleton<Store>();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
