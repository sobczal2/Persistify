using Persistify.Indexes.Common;

namespace Persistify.Indexes.Number;

public class NumberSearchPredicate : ISearchPredicate
{
    public double Min { get; set; }
    public double Max { get; set; }
    public string TypeName { get; set; } = default!;
    public string Path { get; set; } = default!;
}