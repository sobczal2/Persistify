using System.Collections.Generic;
using Persistify.Management.Types.Abstractions;

namespace Persistify.Management.Types.Fts;

public class FtsManagerHit : ITypeManagerHit
{
    public FtsManagerHit(long documentId, float score, List<int> positions)
    {
        DocumentId = documentId;
        Score = score;
        Positions = positions;
    }

    public float Score { get; }
    public List<int> Positions { get; }
    public long DocumentId { get; }
}
