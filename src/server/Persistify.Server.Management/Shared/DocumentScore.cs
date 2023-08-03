namespace Persistify.Server.Management.Shared;

public struct DocumentScore
{
    public int DocumentId { get; set; }
    public float Score { get; set; }

    public DocumentScore(int documentId, float score)
    {
        DocumentId = documentId;
        Score = score;
    }
}
