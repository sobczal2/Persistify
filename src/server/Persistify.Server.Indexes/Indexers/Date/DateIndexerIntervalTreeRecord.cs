using System;

namespace Persistify.Server.Indexes.Indexers.Date;

public class DateIndexerIntervalTreeRecord : IComparable<DateIndexerIntervalTreeRecord>
{
    public int DocumentId { get; set; }
    public DateTime Value { get; set; }

    public int CompareTo(DateIndexerIntervalTreeRecord? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
