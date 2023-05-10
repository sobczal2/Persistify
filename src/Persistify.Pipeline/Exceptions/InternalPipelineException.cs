using System;

namespace Persistify.Pipeline.Exceptions;

public class InternalPipelineException : Exception
{
    public InternalPipelineException(string message = "Internal pipeline error occured.") : base(message)
    {
    }
}