using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Number;

public class NumberManagerHit : ITypeManagerHit
{
    public NumberManagerHit(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; }
}
