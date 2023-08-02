using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Text;

public class TextManagerHit : ITypeManagerHit
{
    public TextManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
