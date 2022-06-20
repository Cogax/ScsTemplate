using Microsoft.AspNetCore.Mvc;

using Planzer.Pak.Messaging.NServiceBus.WebOutbox;

using Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Common;

namespace Poc.Nsb.Outbox.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class MyEntityController : ControllerBase
{
    //[HttpPost]
    //public async Task<IActionResult> Create(
    //    string foo,
    //    bool exception,
    //    [FromServices] WebOutboxMessageSession messageSession,
    //    [FromServices] WriteModelDbContext db)
    //{
    //    //var entity = new MyEntity
    //    //{
    //    //    Id = Guid.NewGuid(),
    //    //    Foo = foo
    //    //};
    //    //await db.MyEntities.AddAsync(entity);
    //    //await messageSession.Publish(new MyEntityCreatedEvent { Id = entity.Id });

    //    //if (exception)
    //    //    throw new Exception("Exception before database commit");

    //    //await db.SaveChangesAsync();
    //    //return new OkObjectResult(entity);
    //    return Ok();
    //}

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<MyEntity>>> GetAll(
    //    [FromServices] PocDbContext db)
    //{
    //    var entities = await db.MyEntities.ToListAsync();
    //    return entities;
    //} 

    //[HttpDelete]
    //public async Task<IActionResult> DeleteAll(
    //    [FromServices] PocDbContext db)
    //{
    //    var entities = await db.MyEntities.ToListAsync();
    //    db.MyEntities.RemoveRange(entities);

    //    await db.SaveChangesAsync();
    //    return Ok();
    //} 
}
