using System;

namespace Persistify.Server.Persistence.Core.Files.Exceptions;

public class FileStructureCorruptedException : Exception
{
    public FileStructureCorruptedException() : base("File structure is corrupted")
    {

    }
}
