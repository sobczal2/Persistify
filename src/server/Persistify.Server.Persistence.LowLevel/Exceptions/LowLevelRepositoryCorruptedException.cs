using System;

namespace Persistify.Server.Persistence.LowLevel.Exceptions;

public class LowLevelRepositoryCorruptedException : Exception
{
    public LowLevelRepositoryCorruptedException() : base("Repository is corrupted")
    {

    }
}
