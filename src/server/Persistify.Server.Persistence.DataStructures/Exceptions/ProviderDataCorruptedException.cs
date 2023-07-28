using System;

namespace Persistify.Server.Persistence.DataStructures.Exceptions;

public class ProviderDataCorruptedException : Exception
{
    public ProviderDataCorruptedException() : base("Provider data is corrupted")
    {

    }
}
