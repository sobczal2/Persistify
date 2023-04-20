using Persistify.Indexes.Common;

namespace Persistify.Indexes.Boolean;

public class BooleanSearchPredicate : ISearchPredicate
{
    public bool Value { get; set; }
}