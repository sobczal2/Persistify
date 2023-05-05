using Persistify.Indexes.Common;

namespace Persistify.Indexes.Boolean;

public class BooleanSearchPredicate : ISearchPredicate
{
    public bool Value { get; set; }
    public string TypeName { get; set; } = default!;
    public string Path { get; set; } = default!;
}