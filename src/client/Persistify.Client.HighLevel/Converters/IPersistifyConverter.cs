using System;
using Persistify.Dtos.Common;

namespace Persistify.Client.HighLevel.Converters;

public interface IPersistifyConverter
{
    Type FromType { get; }
    FieldTypeDto FieldTypeDto { get; }

    object Convert(
        object from
    );

    object ConvertBack(
        object to
    );
}
