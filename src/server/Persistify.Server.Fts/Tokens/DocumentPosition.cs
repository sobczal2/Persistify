using System;

namespace Persistify.Server.Fts.Tokens;

public class DocumentPosition : IComparable<DocumentPosition>
{
    public DocumentPosition(int documentId, int position, float score)
    {
        DocumentId = documentId;
        Position = position;
        Score = score;
    }

    public int DocumentId { get; }
    public int Position { get; }
    public float Score { get; }

    public int CompareTo(DocumentPosition? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        var documentIdComparison = DocumentId.CompareTo(other.DocumentId);
        if (documentIdComparison != 0)
        {
            return documentIdComparison;
        }

        return Position.CompareTo(other.Position);
    }
}
