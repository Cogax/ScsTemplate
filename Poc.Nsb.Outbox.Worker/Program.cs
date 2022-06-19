using Poc.Nsb.Outbox.Infrastructure.Extensions;
using Poc.Nsb.Outbox.Worker;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
builder.Host.AddMessaging("Poc.Nsb.Outbox.Worker", sendOnly: false);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<Store>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
