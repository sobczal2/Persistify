using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Bool;

public class BoolManagerHit : ITypeManagerHit
{
    public BoolManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
