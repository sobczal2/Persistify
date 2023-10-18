using System;
using System.Collections.Generic;

namespace Persistify.Server.Fts.Exceptions;

public class AnalyzerPresetNotFoundException : Exception
{
    public AnalyzerPresetNotFoundException(string presetName, IEnumerable<string> supportedPresetNames)
        : base(
            $"Analyzer preset not found: {presetName}. Supported analyzer presets: {string.Join(", ", supportedPresetNames)}")
    {
    }
}
