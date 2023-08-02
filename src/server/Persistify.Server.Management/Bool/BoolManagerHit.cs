using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Bool;

public class BoolManagerHit : ITypeManagerHit
{
    public BoolManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
