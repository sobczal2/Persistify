using System;

namespace Persistify.Server.Persistence.Core.Exceptions;

public class RepositoryCorruptedException : Exception
{
    public RepositoryCorruptedException() : base("Repository is corrupted")
    {

    }
}
