using Persistify.Diagnostics.Enums;

namespace Persistify.Diagnostics.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PipelineStepAttribute : Attribute
{
    public PipelineStepAttribute(PipelineStepType type)
    {
        Type = type;
    }

    public PipelineStepType Type { get; }
}