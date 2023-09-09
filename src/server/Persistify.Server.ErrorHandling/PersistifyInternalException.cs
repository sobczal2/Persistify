using System;
using System.Diagnostics.CodeAnalysis;

namespace Persistify.Server.ErrorHandling;

public class PersistifyInternalException : Exception
{
    [ExcludeFromCodeCoverage]
    public PersistifyInternalException() : base("Internal error")
    {
    }
}
