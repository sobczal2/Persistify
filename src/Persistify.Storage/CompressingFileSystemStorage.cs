using System.IO.Compression;

namespace Persistify.Storage;

public class CompressingFileSystemStorage : FileSystemStorage
{
    public CompressingFileSystemStorage(string rootPath) : base(rootPath, Compress, Decompress)
    {
    }

    private static Stream Compress(Stream data)
    {
        var compressedStream = new MemoryStream();
        using (var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal, true))
        {
            data.CopyTo(gzipStream);
        }

        compressedStream.Position = 0;
        return compressedStream;
    }

    private static Stream Decompress(Stream data)
    {
        var decompressedStream = new MemoryStream();
        using (var gzipStream = new GZipStream(data, CompressionMode.Decompress, true))
        {
            gzipStream.CopyTo(decompressedStream);
        }

        decompressedStream.Position = 0;
        return decompressedStream;
    }
}