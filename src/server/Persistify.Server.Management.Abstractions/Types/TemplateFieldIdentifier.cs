namespace Persistify.Server.Management.Types.Abstractions;

public struct TemplateFieldIdentifier
{
    public int TemplateId { get; set; }
    public string FieldName { get; set; }

    public TemplateFieldIdentifier(int templateId, string fieldName)
    {
        TemplateId = templateId;
        FieldName = fieldName;
    }
}
