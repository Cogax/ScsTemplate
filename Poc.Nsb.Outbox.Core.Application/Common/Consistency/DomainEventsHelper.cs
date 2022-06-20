using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Application.Common.Consistency;

/// <summary>
/// Adapted from: https://github.com/kgrzybek/efcore-concurrency-handling/blob/master/src/OptimisticConcurrency/DDD.EF.OptimisticConcurrency/Infrastructure/DomainEventsHelper.cs
/// </summary>
public class DomainEventsHelper
{
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> typeFieldCache = new();

    public static List<IDomainEvent> RetrieveAggregatedDomainEvents(IAggregateRoot aggregate)
    {
        return RetrieveAggregatedDomainEvents(aggregate as Entity);
    }

    public static void CleanAggregatedDomainEvents(IAggregateRoot aggregate)
    {
        RetrieveAggregatedDomainEvents(aggregate);
    }

    private static List<IDomainEvent> RetrieveAggregatedDomainEvents(Entity? entity)
    {
        var domainEvents = new List<IDomainEvent>();

        if (entity is null)
        {
            return domainEvents;
        }

        domainEvents.AddRange(entity.DomainEvents);
        entity.ClearDomainEvents();

        var fields = typeFieldCache.GetOrAdd(entity.GetType(), type => type
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Concat(type.BaseType?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<FieldInfo>())
            .ToArray());

        foreach (var field in fields)
        {
            bool isEntity = field.FieldType.IsSubclassOf(typeof(Entity));

            if (isEntity)
            {
                var entityObject = field.GetValue(entity) as Entity;
                domainEvents.AddRange(RetrieveAggregatedDomainEvents(entityObject).ToList());
            }

            if (field.FieldType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(field.FieldType))
            {
                if (field.GetValue(entity) is IEnumerable enumerable)
                {
                    foreach (object? en in enumerable)
                    {
                        if (en is Entity entityItem)
                        {
                            domainEvents.AddRange(RetrieveAggregatedDomainEvents(entityItem));
                        }
                    }
                }
            }
        }

        return domainEvents;
    }
}

