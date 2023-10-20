﻿using System.Collections.Generic;
using Persistify.Dtos.Templates.Common;
using Persistify.Responses.Common;
using ProtoBuf;

namespace Persistify.Responses.Templates;

[ProtoContract]
public class ListTemplatesResponse : IResponse
{
    [ProtoMember(1)]
    public List<TemplateDto> TemplateDtos { get; set; }

    [ProtoMember(2)]
    public int TotalCount { get; set; }

    public ListTemplatesResponse()
    {
        TemplateDtos = new List<TemplateDto>();
    }
}
