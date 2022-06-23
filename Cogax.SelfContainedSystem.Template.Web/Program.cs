using Cogax.SelfContainedSystem.Template.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.BuildWeb();
app.Run();

namespace Cogax.SelfContainedSystem.Template.Web
{
    public partial class WebProgram { }
}
