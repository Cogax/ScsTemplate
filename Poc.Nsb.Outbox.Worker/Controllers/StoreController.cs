using Microsoft.AspNetCore.Mvc;

namespace Poc.Nsb.Outbox.Worker.Controllers;

[ApiController]
[Route("[controller]")]
public class StoreController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll(
        [FromServices] Store store)
    {
        return store.Stack.ToList();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll(
        [FromServices] Store store)
    {
        store.Stack.Clear();
        return Ok();
    }
}
