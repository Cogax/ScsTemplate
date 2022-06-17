using Poc.Nsb.Outbox.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.ConfigureDefaultConfig(builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);
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
app.Run();