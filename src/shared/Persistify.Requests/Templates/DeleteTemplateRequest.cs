﻿using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class DeleteTemplateRequest
{
    [ProtoMember(1)]
    public int TemplateId { get; set; }
}