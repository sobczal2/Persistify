using System.IO;
using System.Runtime.InteropServices;

namespace Persistify.Server.Persistence.Primitives;

public class IntStreamRepository : ByteArrayBasedStreamRepository<int>
{
    public IntStreamRepository(
        Stream stream
    )
        : base(stream, sizeof(int))
    {
    }

    protected override int BytesToValue(
        byte[] bytes
    )
    {
        return MemoryMarshal.Read<int>(bytes);
    }

    protected override byte[] ValueToBytes(
        int value
    )
    {
        var bytes = new byte[sizeof(int)];
        MemoryMarshal.Write(bytes, value);
        return bytes;
    }
}
