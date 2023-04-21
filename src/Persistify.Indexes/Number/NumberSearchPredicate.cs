using Persistify.Indexes.Common;

namespace Persistify.Indexes.Number;

public class NumberSearchPredicate : ISearchPredicate
{
    public double Min { get; set; }
    public double Max { get; set; }
}