using Persistify.Indexes.Common;

namespace Persistify.Indexes.Number;

public class NumberSearchPredicate : ISearchPredicate
{
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
}