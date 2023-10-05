using System;

namespace Persistify.Server.Files.Exceptions;

public class FileStructureCorruptedException : Exception
{
    public FileStructureCorruptedException() : base("File structure is corrupted")
    {
    }
}
