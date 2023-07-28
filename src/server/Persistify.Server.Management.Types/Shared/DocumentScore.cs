namespace Persistify.Server.Management.Types.Shared;

public struct DocumentScore
{
    public long DocumentId { get; set; }
    public float Score { get; set; }

    public DocumentScore(long documentId, float score)
    {
        DocumentId = documentId;
        Score = score;
    }
}
