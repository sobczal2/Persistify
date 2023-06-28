namespace Persistify.Pipelines.Common;

public abstract class PipelineContext
{
    public Guid CorrelationId { get; }

    public PipelineContext(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
}