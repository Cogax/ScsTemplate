using Cogax.SelfContainedSystem.Template.Core.Application.Common.Abstractions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using MediatR;

namespace Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;

public record EmailTodoItemCompletedCommand(TodoItemId TodoItemId) : ICommand;

internal class NotifyTodoItemCompletedCommandHandler : ICommandHandler<EmailTodoItemCompletedCommand>
{
    private readonly ITodoEmailPort _emailPort;

    public NotifyTodoItemCompletedCommandHandler(ITodoEmailPort emailPort)
    {
        _emailPort = emailPort;
    }

    public async Task<Unit> Handle(EmailTodoItemCompletedCommand request, CancellationToken cancellationToken)
    {
        await _emailPort.SendEmail(request.TodoItemId, cancellationToken);
        return Unit.Value;
    }
}
