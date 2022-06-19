using Microsoft.EntityFrameworkCore;

using Poc.Nsb.Outbox.Infrastructure.Extensions;
using Poc.Nsb.Outbox.Infrastructure.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
builder.Host.AddMessaging("Poc.Nsb.Outbox.Web", sendOnly: true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Services.Migrate<PocDbContext>();
app.Run();
