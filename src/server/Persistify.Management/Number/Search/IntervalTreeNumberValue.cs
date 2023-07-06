using System;

namespace Persistify.Management.Number.Search;

public class IntervalTreeNumberValue : IComparable<IntervalTreeNumberValue>, IComparable<double>
{
    public IntervalTreeNumberValue(double value, ulong documentId)
    {
        Value = value;
        DocumentId = documentId;
    }

    public double Value { get; set; }
    public ulong DocumentId { get; set; }

    public int CompareTo(double other)
    {
        return Value.CompareTo(other);
    }

    public int CompareTo(IntervalTreeNumberValue? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
