using System.Collections.Generic;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Requests.Common;
using Persistify.Responses.Documents;
using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class CreateDocumentRequest : IRequest<CreateDocumentResponse>
{
    public CreateDocumentRequest()
    {
        FieldValueDtos = new List<FieldValueDto>();
    }

    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;

    [ProtoMember(2)]
    public List<FieldValueDto> FieldValueDtos { get; set; }
}
