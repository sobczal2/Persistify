using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Text;

public class TextManagerHit : ITypeManagerHit
{
    public TextManagerHit(long documentId)
    {
        DocumentId = documentId;
    }

    public long DocumentId { get; }
}
