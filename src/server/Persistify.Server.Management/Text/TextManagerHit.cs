using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Text;

public class TextManagerHit : ITypeManagerHit
{
    public TextManagerHit(int documentId)
    {
        DocumentId = documentId;
    }

    public int DocumentId { get; }
}
