using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.Number;

public class DoubleToNumberPersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(double);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.Number;
    public object Convert(object from)
    {
        return from;
    }
}
