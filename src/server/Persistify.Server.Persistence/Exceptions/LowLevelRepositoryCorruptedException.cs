using System;

namespace Persistify.Server.Persistence.Exceptions;

public class LowLevelRepositoryCorruptedException : Exception
{
    public LowLevelRepositoryCorruptedException() : base("Repository is corrupted")
    {

    }
}
