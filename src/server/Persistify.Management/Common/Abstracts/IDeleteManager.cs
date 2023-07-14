namespace Persistify.Management.Common.Abstracts;

public interface IDeleteManager
{
    void Delete(string templateName, long documentId);
}
