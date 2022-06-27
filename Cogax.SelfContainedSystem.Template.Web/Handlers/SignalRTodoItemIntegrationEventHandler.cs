using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Web.Handlers;

public class SignalRTodoItemIntegrationEventHandler :
    IHandleMessages<TodoItemAddedIntegrationEvent>,
    IHandleMessages<TodoItemRemovedIntegrationEvent>,
    IHandleMessages<TodoItemsDeletedIntegrationEvent>
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
        var readModel = await _mediator.Send(new GetTodoItemDescriptionQuery(new TodoItemId(message.TodoItemId)));
        await _signalRPublisher.NewTodoItem(readModel);
    }

    public async Task Handle(TodoItemRemovedIntegrationEvent message, IMessageHandlerContext context)
    {
        await _signalRPublisher.RemoveTodoItemdoItem(message.TodoItemId);
    }

    public async Task Handle(TodoItemsDeletedIntegrationEvent message, IMessageHandlerContext context)
    {
        await _signalRPublisher.TodoItemsDeleted();
    }
}
