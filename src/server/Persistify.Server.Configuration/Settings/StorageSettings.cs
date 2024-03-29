﻿using System;
using Persistify.Server.Configuration.Enums;

namespace Persistify.Server.Configuration.Settings;

public class StorageSettings
{
    public const string SectionName = "Storage";

    public string DataPath { get; set; } = default!;
    public StorageType StorageType { get; set; }
    public SerializerType SerializerType { get; set; }
    public TimeSpan IdleFileTimeout { get; set; }
}
