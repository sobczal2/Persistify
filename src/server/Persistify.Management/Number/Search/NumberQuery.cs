using Persistify.Management.Common;

namespace Persistify.Management.Number.Search;

public class NumberQuery : Query
{
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
}
