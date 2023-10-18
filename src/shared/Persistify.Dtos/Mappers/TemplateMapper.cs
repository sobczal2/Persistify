using System.Linq;
using Persistify.Domain.Templates;
using Persistify.Dtos.Templates.Common;

namespace Persistify.Dtos.Mappers;

public static class TemplateMapper
{
    public static TemplateDto Map(Template from)
    {
        var fields = from.Fields.Select(FieldMapper.Map).ToList();
        return new TemplateDto { Name = from.Name, Fields = fields };
    }
}
