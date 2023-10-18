using System.Linq;
using Persistify.Domain.Documents;
using Persistify.Dtos.Documents.Common;

namespace Persistify.Dtos.Mappers;

public static class DocumentMapper
{
    public static DocumentDto Map(Document from)
    {
        var fields = from.FieldValues.Select(FieldValueMapper.Map).ToList();
        return new DocumentDto { Id = from.Id, FieldValues = fields };
    }

    public static Document Map(DocumentDto from)
    {
        var fields = from.FieldValues.Select(FieldValueMapper.Map).ToList();
        return new Document { Id = from.Id, FieldValues = fields };
    }
}
