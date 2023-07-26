using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ProtoBuf;

namespace Persistify.Server.Management.Types.Number;

[ProtoContract]
public class NumberManagerRecord : IComparable<double>
{
    [ProtoMember(1)]
    public double Value { get; set; }

    [ProtoMember(2)]
    public HashSet<long> DocumentIds { get; set; }

    public NumberManagerRecord()
    {
        DocumentIds = new HashSet<long>(0);
    }

    public NumberManagerRecord(double value, HashSet<long> documentIds)
    {
        Value = value;
        DocumentIds = documentIds;
    }

    public NumberManagerRecord(double value, long documentId)
    {
        Value = value;
        DocumentIds = new HashSet<long> { documentId };
    }

    public int CompareTo(double other)
    {
        return Value.CompareTo(other);
    }
}

public class NumberManagerRecordComparer : IComparer<NumberManagerRecord>
{
    public int Compare(NumberManagerRecord? x, NumberManagerRecord? y)
    {
        if (ReferenceEquals(x, y))
        {
            return 0;
        }

        if (ReferenceEquals(null, y))
        {
            return 1;
        }

        if (ReferenceEquals(null, x))
        {
            return -1;
        }

        return x.Value.CompareTo(y.Value);
    }
}
