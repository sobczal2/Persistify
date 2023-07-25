using System;

namespace Persistify.Server.Persistence.Core.Exceptions;

public class RepositoryNotFoundException : Exception
{
    public RepositoryNotFoundException(string repositoryName) : base($"Repository {repositoryName} not found")
    {

    }
}
