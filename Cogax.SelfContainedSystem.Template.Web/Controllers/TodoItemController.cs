using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Cogax.SelfContainedSystem.Template.Web.Controllers;

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
    public async Task<IActionResult> Create(string label)
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

    [HttpGet]
    public async Task<IEnumerable<TodoItemDescription>> GetAll()
    {
        var query = new GetAllTodoItemsQuery();
        var result = await _mediator.Send(query, HttpContext.RequestAborted);
        return result;
    }

    [HttpPut]
    public async Task<IActionResult> Complete(Guid id)
    {
        var command = new CompleteTodoItemCommand(new TodoItemId(id));
        await _mediator.Send(command, HttpContext.RequestAborted);
        return Ok();
    }
}
