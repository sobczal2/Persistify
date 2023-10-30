using System;

namespace Persistify.Server.Persistence.Exceptions;

public class RepositoryCorruptedException : Exception
{
    public RepositoryCorruptedException()
        : base("Repository is corrupted") { }
}
