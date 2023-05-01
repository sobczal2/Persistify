namespace Persistify.Monitor.Database.Domain;

public class PipelineEvent : Entity
{
    public Guid CorrelationId { get; set; }
    public string PipelineName { get; set; } = default!;
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Success { get; set; }
    public string? FaultedStepName { get; set; }
}