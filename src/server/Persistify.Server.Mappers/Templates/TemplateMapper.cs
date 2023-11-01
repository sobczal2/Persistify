using Persistify.Dtos.Templates.Common;
using Persistify.Helpers.Collections;
using Persistify.Server.Domain.Templates;

namespace Persistify.Server.Mappers.Templates;

public static class TemplateMapper
{
    public static TemplateDto ToDto(this Template template)
    {
        return new TemplateDto { Name = template.Name, Fields = template.Fields.ListSelect(x => x.ToDto()) };
    }
}
