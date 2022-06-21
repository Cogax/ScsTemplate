namespace Cogax.SelfContainedSystem.Template.Core.Domain.Common;

public interface IEntity
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public abstract class Entity : IEntity, IComparable, IComparable<Entity>
{
    private List<IDomainEvent> domainEvents;

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        domainEvents.AsReadOnly();

    protected Entity()
    {
        domainEvents = new List<IDomainEvent>();
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents ??= new List<IDomainEvent>();

        domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents?.Clear();
    }

    protected void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    // https://github.com/vkhorikov/CSharpFunctionalExtensions/blob/master/CSharpFunctionalExtensions/ValueObject/ValueObject.cs
    private int? _cachedHashCode;

    protected abstract IEnumerable<object> GetIdentityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null)
            return false;

        if (GetUnproxiedType(this) != GetUnproxiedType(obj))
            return false;

        var entity = (Entity)obj;

        return GetIdentityComponents().SequenceEqual(entity.GetIdentityComponents());
    }

    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = GetIdentityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return (current * 23) + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        return _cachedHashCode.Value;
    }

    public int CompareTo(object? obj)
    {
        Type thisType = GetUnproxiedType(this);
        Type otherType = GetUnproxiedType(obj!);

        if (thisType != otherType)
            return string.Compare(thisType.ToString(), otherType.ToString(), StringComparison.Ordinal);

        var other = (Entity)obj!;

        object[] components = GetIdentityComponents().ToArray();
        object[] otherComponents = other.GetIdentityComponents().ToArray();

        for (int i = 0; i < components.Length; i++)
        {
            int comparison = CompareComponents(components[i], otherComponents[i]);
            if (comparison != 0)
                return comparison;
        }

        return 0;
    }

    private int CompareComponents(object object1, object object2)
    {
        if (object1 is null && object2 is null)
            return 0;

        if (object1 is null)
            return -1;

        if (object2 is null)
            return 1;

        if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
            return comparable1.CompareTo(comparable2);

        return object1.Equals(object2) ? 0 : -1;
    }

    public virtual int CompareTo(Entity? other)
    {
        return CompareTo(other as object);
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    private static Type GetUnproxiedType(object obj)
    {
        const string EFCoreProxyPrefix = "Castle.Proxies.";
        const string NHibernateProxyPostfix = "Proxy";

        Type type = obj.GetType();
        string typeString = type.ToString();

        if ((typeString.Contains(EFCoreProxyPrefix) || typeString.EndsWith(NHibernateProxyPostfix)) && type.BaseType != null)
            return type.BaseType;

        return type;
    }
}
