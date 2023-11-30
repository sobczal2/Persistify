using System;

namespace Persistify.Server.Indexes.Indexers.Number;

public class NumberIndexerIntervalTreeRecord : IComparable<NumberIndexerIntervalTreeRecord>
{
    public int DocumentId { get; set; }
    public double Value { get; set; }

    public int CompareTo(
        NumberIndexerIntervalTreeRecord? other
    )
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
