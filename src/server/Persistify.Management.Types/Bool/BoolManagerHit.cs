using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Bool;

public class BoolManagerHit : ITypeManagerHit
{
    public long DocumentId { get; }

    public BoolManagerHit(long documentId)
    {
        DocumentId = documentId;
    }
}
