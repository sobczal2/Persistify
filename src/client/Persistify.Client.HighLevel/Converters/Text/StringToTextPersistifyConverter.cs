using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters.Text;

public class StringToTextPersistifyConverter : IPersistifyConverter
{
    public Type FromType => typeof(string);
    public FieldTypeDto FieldTypeDto => FieldTypeDto.Text;
    public object Convert(object from)
    {
        return from;
    }
}
