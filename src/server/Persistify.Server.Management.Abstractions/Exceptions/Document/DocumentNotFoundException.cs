using System;

namespace Persistify.Server.Management.Abstractions.Exceptions.Document;

public class DocumentNotFoundException : Exception
{
    public DocumentNotFoundException(long id) : base($"Document with id {id} not found")
    {

    }
}
