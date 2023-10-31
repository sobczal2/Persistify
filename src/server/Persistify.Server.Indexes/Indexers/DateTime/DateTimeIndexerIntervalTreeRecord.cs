using System;

namespace Persistify.Server.Indexes.Indexers.DateTime;

public class DateTimeIndexerIntervalTreeRecord : IComparable<DateTimeIndexerIntervalTreeRecord>
{
    public int DocumentId { get; set; }
    public System.DateTime Value { get; set; }

    public int CompareTo(DateTimeIndexerIntervalTreeRecord? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
