using Persistify.Server.Management.Types.Abstractions;

namespace Persistify.Server.Management.Types.Text;

public class TextManagerHit : ITypeManagerHit
{
    public TextManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
