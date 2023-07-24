namespace Persistify.Server.Pipelines.Exceptions;

public class RollbackFailedException : PipelineException
{
    public RollbackFailedException() : base("Rollback failed")
    {
    }

    public RollbackFailedException(string message) : base(message)
    {
    }
}
