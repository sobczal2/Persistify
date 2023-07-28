using Persistify.Domain.Templates;

namespace Persistify.Server.Pipelines.Common.Contexts;

public interface IContextWithTemplate
{
    public int TemplateId { get; set; }
    public Template? Template { get; set; }
}
