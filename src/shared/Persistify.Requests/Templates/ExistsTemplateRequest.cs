﻿using ProtoBuf;

namespace Persistify.Requests.Templates;

[ProtoContract]
public class ExistsTemplateRequest
{
    [ProtoMember(1)]
    public string TemplateName { get; set; } = default!;
}