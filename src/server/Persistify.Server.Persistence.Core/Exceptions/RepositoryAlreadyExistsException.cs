using System;

namespace Persistify.Server.Persistence.Core.Exceptions;

public class RepositoryAlreadyExistsException : Exception
{
    public RepositoryAlreadyExistsException(string repositoryName) : base($"Repository {repositoryName} already exists")
    {

    }
}
