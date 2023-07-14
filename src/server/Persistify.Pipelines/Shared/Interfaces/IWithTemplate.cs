namespace Persistify.Pipelines.Shared.Interfaces;

public interface IWithTemplate
{
    Protos.Templates.Shared.Template? Template { get; set; }
    string? TemplateName { get; set; }
}
