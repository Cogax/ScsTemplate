using Cogax.SelfContainedSystem.Template.Worker;

var builder = WebApplication.CreateBuilder(args);
var app = builder.BuildWorker();
app.Run();

namespace Cogax.SelfContainedSystem.Template.Worker
{
    public partial class WorkerProgram { }
}
