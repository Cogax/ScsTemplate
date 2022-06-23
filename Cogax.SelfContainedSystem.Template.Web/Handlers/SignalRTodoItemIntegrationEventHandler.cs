using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Contracts;
using Cogax.SelfContainedSystem.Template.Web.SignalR;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Web.Handlers;

public class SignalRTodoItemIntegrationEventHandler :
    IHandleMessages<TodoItemCompletedIntegrationEvent>,
    IHandleMessages<TodoItemAddedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ISignalRPublisher _signalRPublisher;

    public SignalRTodoItemIntegrationEventHandler(
        IMediator mediator,
        ISignalRPublisher signalRPublisher)
    {
        _mediator = mediator;
        _signalRPublisher = signalRPublisher;
    }

    public async Task Handle(TodoItemAddedIntegrationEvent message, IMessageHandlerContext context)
    {
        
    }

    public async Task Handle(TodoItemCompletedIntegrationEvent message, IMessageHandlerContext context)
    {
        var readModel = await _mediator.Send(new GetTodoItemDescriptionQuery(new TodoItemId(message.TodoItemId)));
        await _signalRPublisher.PublishTodoItem(readModel);
    }
}
