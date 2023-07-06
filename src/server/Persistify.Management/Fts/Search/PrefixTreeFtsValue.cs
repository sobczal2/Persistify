using System;

namespace Persistify.Management.Fts.Search;

public struct PrefixTreeFtsValue
{
    public ulong DocumentId { get; set; }
    public float TermFrequency { get; set; }
    public PrefixTreeValueFlags Flags { get; set; }

    public PrefixTreeFtsValue(ulong documentId, float termFrequency, PrefixTreeValueFlags flags)
    {
        DocumentId = documentId;
        TermFrequency = termFrequency;
        Flags = flags;
    }
}

[Flags]
public enum PrefixTreeValueFlags
{
    Exact = 0,
    Suffix = 1
}
