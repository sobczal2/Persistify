using System;

namespace Persistify.Pipelines.Common;

public abstract class PipelineContext
{
    public PipelineContext(Guid correlationId)
    {
        CorrelationId = correlationId;
    }

    public Guid CorrelationId { get; }
}
