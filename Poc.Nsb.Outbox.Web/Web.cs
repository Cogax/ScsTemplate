using Poc.Nsb.Outbox.Infrastructure.Extensions;
using Poc.Nsb.Outbox.Infrastructure.Model;

namespace Poc.Nsb.Outbox.Web;

public static class Web
{
    public static WebApplication BuildWeb(this WebApplicationBuilder builder)
    {
        builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
        builder.Host.AddMessaging("Poc.Nsb.Outbox.Web", enableSendOnly: true, enableWebOutbox: true, enableNsbOutbox: true);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddInfrastructure(builder.Configuration);

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseAuthorization();
        app.MapControllers();
        app.Services.Migrate<PocDbContext>();

        return app;
    }
}
