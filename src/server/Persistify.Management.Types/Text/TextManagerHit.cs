using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Text;

public class TextManagerHit : ITypeManagerHit
{
    public long DocumentId { get; }

    public TextManagerHit(long documentId)
    {
        DocumentId = documentId;
    }
}
