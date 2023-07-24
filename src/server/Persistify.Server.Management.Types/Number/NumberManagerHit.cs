using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Number;

public class NumberManagerHit : ITypeManagerHit
{
    public NumberManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
