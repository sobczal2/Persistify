using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Number;

public class NumberManagerHit : ITypeManagerHit
{
    public long DocumentId { get; }

    public NumberManagerHit(long documentId)
    {
        DocumentId = documentId;
    }
}
