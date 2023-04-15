using System;

namespace Persistify.Pipeline.Exceptions;

public class ResponseNotYetReadyException : Exception
{
    public ResponseNotYetReadyException() : base("Response has not yet been set.")
    {
    }
}