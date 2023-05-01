using System;

namespace Persistify.Pipeline.Exceptions;

public class ResponseAlreadySetException : Exception
{
    public ResponseAlreadySetException() : base("Response has already been set.")
    {
    }
}