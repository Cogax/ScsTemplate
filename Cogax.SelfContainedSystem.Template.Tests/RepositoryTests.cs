using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Tests.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cogax.SelfContainedSystem.Template.Tests;

[TestClass]
public class RepositoryTests : WebApplicationTestBase
{
    [TestMethod]
    public async Task OptimisticConcurrency_WhenParallelModificationOnTheSameAggregateRoot_ThenConcurrencyException()
    {
        // Arrange
        await WebClient.PostAsync("/TodoItem?label=test", null);
        var response = await WebClient.GetAsync("/TodoItem");
        var todoItemDto = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

        // Act
        var operation1 = Task.Run(async () =>
        {
            using var scope = Web.Services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.ExecuteBusinessOperation(async token =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<ITodoItemRepository>();
                var todoItem1 = await repository.GetById(new TodoItemId(todoItemDto.Id), token);
                todoItem1.Complete();
                await Task.Delay(TimeSpan.FromSeconds(2));
                return 1;
            }, CancellationToken.None);
        });

        var operation2 = Task.Run(async () =>
        {
            using var scope = Web.Services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unitOfWork.ExecuteBusinessOperation(async token =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<ITodoItemRepository>();
                var todoItem1 = await repository.GetById(new TodoItemId(todoItemDto.Id), token);
                todoItem1.Complete();
                return 1;
            }, CancellationToken.None);
        });

        try
        {
            await Task.WhenAll(operation1, operation2);
        }
        catch (AggregateRootConcurrencyException ex)
        {
            return;
        }

        Assert.Fail($"{nameof(AggregateRootConcurrencyException)} expected but no exception thrown!");
    }
}
