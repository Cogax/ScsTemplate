using System.Data;

using AutoMapper;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Repositories;

public class TodoItemRepositoryWithLegacyDb : ITodoItemRepository
{
    private readonly ReadModelDbContext _readDb;
    private readonly WriteModelDbContext _writeDb;

    public TodoItemRepositoryWithLegacyDb(ReadModelDbContext readDb, WriteModelDbContext writeDb)
    {
        _readDb = readDb;
        _writeDb = writeDb;
    }

    public class LegacyMappingProfile : Profile
    {
        public LegacyMappingProfile()
        {
            ShouldMapField = fieldInfo => true;
            ShouldMapProperty = propertyInfo => true;
            CreateMap<TodoItemId, Guid>().ConvertUsing<TodoItemIdToGuidConverter>();
            CreateMap<Label, string>().ConvertUsing<LabelToStringConverter>();
            CreateMap<TodoItem, Generated.TodoItems>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Label, opt => opt.MapFrom("label"))
                .ForMember(dest => dest.Completed, opt => opt.MapFrom("completed"))
                .ForMember(dest => dest.Removed, opt => opt.MapFrom("removed"))
                .ForMember(dest => dest.Version, opt => opt.MapFrom("version"))
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        }
    }

    public class TodoItemIdToGuidConverter : ITypeConverter<TodoItemId, Guid>
    {
        public Guid Convert(TodoItemId source, Guid destination, ResolutionContext context)
        {
            return source.Value;
        }
    }

    public class LabelToStringConverter : ITypeConverter<Label, string>
    {
        public string Convert(Label source, string destination, ResolutionContext context)
        {
            return source.Value;
        }
    }

    public async Task Add(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new LegacyMappingProfile()))
            .CreateMapper();

        var mappedTodoItem = mapper.Map<Generated.TodoItems>(todoItem);
        await _readDb.TodoItems.AddAsync(mappedTodoItem, cancellationToken);
    }

    public async Task<TodoItem> GetById(TodoItemId id, CancellationToken cancellationToken = default)
    {
        var todoItem = await _writeDb.TodoItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (todoItem is null)
            throw new AggregateRootNotFoundException(nameof(TodoItem),
                new KeyValuePair<string, object>(nameof(TodoItem.Id), id));

        return todoItem;
    }

    public async Task ClearAll(CancellationToken cancellationToken = default)
    {
        _writeDb.TodoItems.RemoveRange(await _writeDb.TodoItems.ToListAsync(cancellationToken));
    }

    public async Task VerifyConnectionOpen(CancellationToken cancellationToken = default)
    {
        var canConnect = await _writeDb.Database.CanConnectAsync(cancellationToken);
        var connection = _writeDb.Database.GetDbConnection();

        if (!canConnect || connection.State != ConnectionState.Open)
            throw new Exception("Connection Closed!");
    }
}
