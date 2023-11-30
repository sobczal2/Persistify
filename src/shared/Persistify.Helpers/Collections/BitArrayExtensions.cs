using System.Collections;

namespace Persistify.Helpers.Collections;

public static class BitArrayExtensions
{
    public static void SetEnsureCapacity(
        this BitArray bitArray,
        int index,
        bool value
    )
    {
        if (bitArray.Length <= index)
        {
            bitArray.Length = index + 1;
        }

        bitArray.Set(index, value);
    }
}
