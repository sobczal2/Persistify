﻿using System;
using System.Collections.Generic;

namespace Persistify.Server.Fts.Analysis.Exceptions;

public class UnsupportedPresetException : Exception
{
    public UnsupportedPresetException(string presetName, IEnumerable<string> supportedPresetNames)
        : base($"Unsupported preset: {presetName}. Supported presets: {string.Join(", ", supportedPresetNames)}")
    {
    }
}