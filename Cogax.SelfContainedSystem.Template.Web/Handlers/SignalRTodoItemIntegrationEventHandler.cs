using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Web.Handlers;

public class SignalRTodoItemIntegrationEventHandler :
    IHandleMessages<TodoItemCompletedIntegrationEvent>,
    IHandleMessages<TodoItemAddedIntegrationEvent>,
    IHandleMessages<TodoItemRemovedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ISignalRPublisher _signalRPublisher;
    private readonly IChaosMonkey _chaosMonkey;

    public SignalRTodoItemIntegrationEventHandler(
        IMediator mediator,
        ISignalRPublisher signalRPublisher,
        IChaosMonkey chaosMonkey)
    {
        _mediator = mediator;
        _signalRPublisher = signalRPublisher;
        _chaosMonkey = chaosMonkey;
    }

    public async Task Handle(TodoItemAddedIntegrationEvent message, IMessageHandlerContext context)
    {
        _chaosMonkey.OnWebNsbHandle();
        var readModel = await _mediator.Send(new GetTodoItemDescriptionQuery(new TodoItemId(message.TodoItemId)));
        await _signalRPublisher.PublishTodoItem(readModel);
    }

    public async Task Handle(TodoItemCompletedIntegrationEvent message, IMessageHandlerContext context)
    {
        _chaosMonkey.OnWebNsbHandle();
        var readModel = await _mediator.Send(new GetTodoItemDescriptionQuery(new TodoItemId(message.TodoItemId)));
        await _signalRPublisher.PublishTodoItem(readModel);
    }

    public async Task Handle(TodoItemRemovedIntegrationEvent message, IMessageHandlerContext context)
    {
        _chaosMonkey.OnWebNsbHandle();
        await _signalRPublisher.PublishTodoItem(null);
    }
}
