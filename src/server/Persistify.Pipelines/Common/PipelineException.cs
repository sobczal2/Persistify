using System;

namespace Persistify.Pipelines.Common;

public class PipelineException : Exception
{
    public PipelineException(string message) : base(message)
    {

    }

    public PipelineException() : base("Pipeline failed")
    {

    }
}
