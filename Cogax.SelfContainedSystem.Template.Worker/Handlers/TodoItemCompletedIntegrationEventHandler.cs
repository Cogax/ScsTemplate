using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Worker.Handlers;

public class TodoItemCompletedIntegrationEventHandler : IHandleMessages<TodoItemCompletedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IChaosMonkey _chaosMonkey;

    public TodoItemCompletedIntegrationEventHandler(
        IMediator mediator,
        IChaosMonkey chaosMonkey)
    {
        _mediator = mediator;
        _chaosMonkey = chaosMonkey;
    }

    public async Task Handle(TodoItemCompletedIntegrationEvent message, IMessageHandlerContext context)
    {
        _chaosMonkey.OnWorkerNsbHandle();
        await _mediator.Send(new EmailTodoItemCompletedCommand(new TodoItemId(message.TodoItemId)));
    }
}
