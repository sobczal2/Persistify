using System;

namespace Persistify.Server.Persistence.Core.Exceptions;

public class RepositoryFactoryInternalException : Exception
{
    public RepositoryFactoryInternalException() : base("Repository factory internal exception")
    {

    }
}
