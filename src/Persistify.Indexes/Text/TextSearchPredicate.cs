using Persistify.Indexes.Common;

namespace Persistify.Indexes.Text;

public class TextSearchPredicate : ISearchPredicate
{
    public string Value { get; set; } = default!;
    public string TypeName { get; set; } = default!;
    public string Path { get; set; } = default!;
    public bool CaseSensitive { get; set; } = true;
    public bool Exact { get; set; } = false;
}