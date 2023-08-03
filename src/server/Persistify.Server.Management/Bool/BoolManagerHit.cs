using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Bool;

public class BoolManagerHit : ITypeManagerHit
{
    public BoolManagerHit(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; }
}
