﻿using System;
using System.IO;

namespace Persistify.Server.Serialization;

public interface ISerializer
{
    ReadOnlyMemory<byte> Serialize<T>(T obj);
    T Deserialize<T>(ReadOnlyMemory<byte> bytes);
}
