using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Migrations;
using Cogax.SelfContainedSystem.Template.Web;
Migrator.Migrate();
var builder = WebApplication.CreateBuilder(args);
var app = builder.BuildWeb();
app.Run();

namespace Cogax.SelfContainedSystem.Template.Web
{
    public partial class WebProgram { }
}
