using System;

namespace Persistify.Server.Errors;

public class PersistifyInternalException : Exception
{
    public PersistifyInternalException() : base("Internal error")
    {
    }
}
