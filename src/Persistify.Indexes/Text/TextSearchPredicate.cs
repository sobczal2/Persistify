using Persistify.Indexes.Common;

namespace Persistify.Indexes.Text;

public class TextSearchPredicate : ISearchPredicate
{
    public string Value { get; set; }
}