using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NServiceBus;
using NServiceBus.Outbox;

using Planzer.Pak.Messaging.NServiceBus.WebOutbox;

using Poc.Nsb.Outbox.Core;
using Poc.Nsb.Outbox.Infrastructure.Events;
using Poc.Nsb.Outbox.Infrastructure.Model;

namespace Poc.Nsb.Outbox.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class MyEntityController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        string foo,
        bool exception,
        [FromServices] WebOutboxMessageSession messageSession,
        [FromServices] PocDbContext db)
    {
        var entity = new MyEntity
        {
            Id = Guid.NewGuid(),
            Foo = foo
        };
        await db.MyEntities.AddAsync(entity);
        await messageSession.Publish(new MyEntityCreatedEvent { Id = entity.Id });

        if (exception)
            throw new Exception("Exception before database commit");

        await db.SaveChangesAsync();
        return new OkObjectResult(entity);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MyEntity>>> GetAll(
        [FromServices] PocDbContext db)
    {
        var entities = await db.MyEntities.ToListAsync();
        return entities;
    } 

    [HttpDelete]
    public async Task<IActionResult> DeleteAll(
        [FromServices] PocDbContext db)
    {
        var entities = await db.MyEntities.ToListAsync();
        db.MyEntities.RemoveRange(entities);

        await db.SaveChangesAsync();
        return Ok();
    } 
}
