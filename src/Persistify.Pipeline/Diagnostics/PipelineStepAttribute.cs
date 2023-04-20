using System;

namespace Persistify.Pipeline.Diagnostics;

[AttributeUsage(AttributeTargets.Class)]
public class PipelineStepAttribute : Attribute
{
    public PipelineStepAttribute(PipelineStepType pipelineStepType)
    {
        PipelineStepType = pipelineStepType;
    }

    public PipelineStepType PipelineStepType { get; }
}