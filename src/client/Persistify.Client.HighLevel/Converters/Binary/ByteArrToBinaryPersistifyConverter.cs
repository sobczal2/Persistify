using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.Binary;

public class ByteArrToBinaryPersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(byte[]);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.Binary;
    public object Convert(object from)
    {
        return from;
    }

    public object ConvertBack(object to)
    {
        return to;
    }
}
