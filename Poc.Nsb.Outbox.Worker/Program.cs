using Poc.Nsb.Outbox.Worker;

var builder = WebApplication.CreateBuilder(args);
var app = builder.BuildWorker();
app.Run();
