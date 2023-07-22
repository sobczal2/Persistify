using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Number;

public class NumberManagerHit : ITypeManagerHit
{
    public NumberManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
