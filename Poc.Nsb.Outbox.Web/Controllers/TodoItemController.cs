using MediatR;

using Microsoft.AspNetCore.Mvc;

using Poc.Nsb.Outbox.Core.Application.Todo.Commands;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodoItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(string label, bool exceptionBeforeSave = false)
    {
        var command = new AddTodoItemCommand(new TodoItemId(Guid.NewGuid()), new Label(label));
        await _mediator.Send(command, HttpContext.RequestAborted);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAll()
    {
        var command = new ClearTodoItemsCommand();
        await _mediator.Send(command, HttpContext.RequestAborted);
        return Ok();
    }
}
