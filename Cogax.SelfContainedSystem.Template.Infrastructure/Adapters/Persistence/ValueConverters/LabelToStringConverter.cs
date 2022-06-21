using System.Data;

using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using Dapper;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ValueConverters;

// EF Core
public class LabelToStringConverter : ValueConverter<Label, string>
{
    public LabelToStringConverter() : base(
        v => v.Value,
        v => new Label(v))
    { }
}

// Dapper
internal sealed class LabelToStringTypeHandler : SqlMapper.TypeHandler<Label>
{
    public override void SetValue(IDbDataParameter parameter, Label value)
    {
        parameter.Value = value.Value;
    }

    public override Label Parse(object v)
    {
        return new Label((string)v);
    }
}
