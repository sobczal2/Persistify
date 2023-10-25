using System.Collections.Generic;
using Persistify.Dtos.Documents.Common;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Server.Domain.Documents;

namespace Persistify.Server.Mappers.Documents;

public static class DocumentMapper
{
    public static DocumentDto ToDto(this Document document)
    {
        var fieldValues = new List<FieldValueDto>(document.FieldValues.Count);

        foreach (var fieldValue in document.FieldValues)
        {
            fieldValues.Add(fieldValue.ToDto());
        }

        return new DocumentDto { Id = document.Id, FieldValueDtos = fieldValues };
    }
}
