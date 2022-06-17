using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Poc.Nsb.Outbox.Core;
using Poc.Nsb.Outbox.Infrastructure.Model;

namespace Poc.Nsb.Outbox.Controllers;

[ApiController]
[Route("[controller]")]
public class MyEntityController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MyEntity>>> Get(
        [FromServices] PocDbContext db) => await db.MyEntities.ToListAsync();
}
