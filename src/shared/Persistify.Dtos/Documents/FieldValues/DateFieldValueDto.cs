using System;

namespace Persistify.Dtos.Documents.FieldValues;

public class DateFieldValueDto : FieldValueDto
{
    public DateTime Value { get; set; }
}
