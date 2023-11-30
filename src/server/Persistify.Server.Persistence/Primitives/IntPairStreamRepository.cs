using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Persistify.Server.Persistence.Primitives;

public class IntPairStreamRepository : ByteArrayBasedStreamRepository<(int, int)>
{
    public IntPairStreamRepository(
        Stream stream
    )
        : base(stream, sizeof(int) * 2)
    {
    }

    protected override (int, int) BytesToValue(
        byte[] bytes
    )
    {
        return (
            MemoryMarshal.Read<int>(bytes.AsSpan(0, sizeof(int))),
            MemoryMarshal.Read<int>(bytes.AsSpan(sizeof(int), sizeof(int)))
        );
    }

    protected override byte[] ValueToBytes(
        (int, int) value
    )
    {
        var bytes = new byte[sizeof(int) * 2];
        MemoryMarshal.Write(bytes.AsSpan(0, sizeof(int)), value.Item1);
        MemoryMarshal.Write(bytes.AsSpan(sizeof(int), sizeof(int)), value.Item2);
        return bytes;
    }
}
