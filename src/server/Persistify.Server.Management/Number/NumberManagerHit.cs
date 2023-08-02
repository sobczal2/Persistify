using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Number;

public class NumberManagerHit : ITypeManagerHit
{
    public NumberManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
