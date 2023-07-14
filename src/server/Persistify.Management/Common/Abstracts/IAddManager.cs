namespace Persistify.Management.Common.Abstracts;

public interface IAddManager
{
    void Add(string templateName, Protos.Documents.Shared.Document document, long documentId);
}
