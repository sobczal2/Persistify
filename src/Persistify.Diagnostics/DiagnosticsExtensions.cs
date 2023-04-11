using System;
using System.Reflection;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;

namespace Persistify.Diagnostics;

public static class DiagnosticsExtensions
{
    public static PipelineStepType GetPipelineStepType(this Type type)
    {
        var attribute = type.GetCustomAttribute<PipelineStepAttribute>();
        return attribute?.Type ?? PipelineStepType.Unknown;
    }
}