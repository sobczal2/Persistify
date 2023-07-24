using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Bool;

public class BoolManagerHit : ITypeManagerHit
{
    public BoolManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
