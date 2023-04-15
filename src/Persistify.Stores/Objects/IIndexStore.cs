namespace Persistify.Stores.Objects;

public interface IIndexStore
{
    public void AddTokens(string typeName, string[] tokens, long documentId);
}