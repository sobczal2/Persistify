using System;

namespace Persistify.Server.Indexes.Indexers.Text;

public class TextIndexerIntervalTreeRecord : IComparable<TextIndexerIntervalTreeRecord>
{
    public int DocumentId { get; set; }
    public string Value { get; set; } = default!;

    public int CompareTo(
        TextIndexerIntervalTreeRecord? other
    )
    {
        if (other == null)
        {
            return 1;
        }

        return String.Compare(Value, other.Value, StringComparison.Ordinal);
    }
}
