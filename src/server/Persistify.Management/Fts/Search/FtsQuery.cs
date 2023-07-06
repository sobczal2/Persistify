using Persistify.Management.Common;

namespace Persistify.Management.Fts.Search;

public class FtsQuery : Query
{
    public string Value { get; set; } = default!;
    public bool Exact { get; set; }
    public bool CaseSensitive { get; set; }
}
