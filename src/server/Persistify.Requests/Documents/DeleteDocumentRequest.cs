﻿using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class DeleteDocumentRequest
{
    [ProtoMember(1)]
    public int TemplateId { get; set; }

    [ProtoMember(2)]
    public long DocumentId { get; set; }
}
