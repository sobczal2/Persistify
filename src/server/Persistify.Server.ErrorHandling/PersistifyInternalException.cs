using System;

namespace Persistify.Server.ErrorHandling;

public class PersistifyInternalException : Exception
{
    public PersistifyInternalException() : base("Internal error")
    {
    }
}
