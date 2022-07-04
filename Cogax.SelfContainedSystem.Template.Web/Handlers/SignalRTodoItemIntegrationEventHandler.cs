using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Queries;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Contracts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;

using Hangfire;

using MediatR;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Web.Handlers;

public class SignalRTodoItemIntegrationEventHandler :
    IHandleMessages<TodoItemAddedIntegrationEvent>,
    IHandleMessages<TodoItemRemovedIntegrationEvent>,
    IHandleMessages<TodoItemsDeletedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IChaosMonkey _chaosMonkey;
    private readonly ISignalRPublisher _signalRPublisher;

    public SignalRTodoItemIntegrationEventHandler(
        IMediator mediator,
        IBackgroundJobClient backgroundJobClient,
        IChaosMonkey chaosMonkey,
        ISignalRPublisher signalRPublisher)
    {
        _mediator = mediator;
        _backgroundJobClient = backgroundJobClient;
        _chaosMonkey = chaosMonkey;
        _signalRPublisher = signalRPublisher;
    }

    public async Task Handle(TodoItemAddedIntegrationEvent message, IMessageHandlerContext context)
    {
        var readModel = await _mediator.Send(new GetTodoItemDescriptionQuery(new TodoItemId(message.TodoItemId)));

        _chaosMonkey.OnNsbHandleTodoItemAdded();
        
        _backgroundJobClient.Enqueue<ISignalRPublisher>(publisher => publisher.NewTodoItem(readModel));
    }

    public async Task Handle(TodoItemRemovedIntegrationEvent message, IMessageHandlerContext context)
    {
        _backgroundJobClient.Enqueue<ISignalRPublisher>(publisher => publisher.RemoveTodoItemdoItem(message.TodoItemId));
    }

    public async Task Handle(TodoItemsDeletedIntegrationEvent message, IMessageHandlerContext context)
    {
        _backgroundJobClient.Enqueue<ISignalRPublisher>(publisher => publisher.TodoItemsDeleted());
    }
}
