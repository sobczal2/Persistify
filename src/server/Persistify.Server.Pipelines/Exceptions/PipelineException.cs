using System;

namespace Persistify.Server.Pipelines.Exceptions;

public class PipelineException : Exception
{
    public PipelineException(string message) : base(message)
    {
    }

    public PipelineException() : base("Pipeline failed")
    {
    }
}
