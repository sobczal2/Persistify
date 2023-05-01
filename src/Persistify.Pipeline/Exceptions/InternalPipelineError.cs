using System;

namespace Persistify.Pipeline.Exceptions;

public class InternalPipelineError : Exception
{
    public InternalPipelineError(string message = "Internal pipeline error occured.") : base(message)
    {
    }
}