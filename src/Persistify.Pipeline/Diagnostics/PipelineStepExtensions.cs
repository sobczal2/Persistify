using System;
using Google.Apis.Util;

namespace Persistify.Pipeline.Diagnostics;

public static class PipelineStepExtensions
{
    public static PipelineStepType GetPipelineStep(this Type type)
    {
        return type.GetCustomAttribute<PipelineStepAttribute>().PipelineStepType;
    }
}