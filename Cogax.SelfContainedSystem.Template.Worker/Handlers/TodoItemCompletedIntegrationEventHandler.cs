using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Worker.Handlers;

public class TodoItemCompletedIntegrationEventHandler : IHandleMessages<TodoItemCompletedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public TodoItemCompletedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(TodoItemCompletedIntegrationEvent message, IMessageHandlerContext context)
    {
        await _mediator.Send(new EmailTodoItemCompletedCommand(new TodoItemId(message.TodoItemId)));
    }
}
