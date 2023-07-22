﻿using System.Collections.Generic;
using Persistify.Domain.Documents;
using ProtoBuf;

namespace Persistify.Requests.Documents;

[ProtoContract]
public class IndexDocumentRequest
{
    public IndexDocumentRequest()
    {
        TextFieldValues = new List<TextFieldValue>(0);
        NumberFieldValues = new List<NumberFieldValue>(0);
        BoolFieldValues = new List<BoolFieldValue>(0);
    }

    [ProtoMember(1)]
    public int TemplateId { get; set; }

    [ProtoMember(2)]
    public List<TextFieldValue> TextFieldValues { get; set; } = default!;

    [ProtoMember(3)]
    public List<NumberFieldValue> NumberFieldValues { get; set; } = default!;

    [ProtoMember(4)]
    public List<BoolFieldValue> BoolFieldValues { get; set; } = default!;
}
