using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions.Document;
using Persistify.Server.Management.Abstractions.Exceptions.Templates;

namespace Persistify.Server.Management.Domain;

public class DocumentManager : IDocumentManager
{
    private readonly ITemplateManager _templateManager;
    private readonly IdentifierManager _identifierManager;

    public DocumentManager(
        ITemplateManager templateManager,
        IdentifierManager identifierManager
        )
    {
        _templateManager = templateManager;
        _identifierManager = identifierManager;
    }
    public async ValueTask AddAsync(int templateId, Document document)
    {
        var template = await _templateManager.GetAsync(templateId);

        if (template is null)
        {
            throw new TemplateNotFoundException(templateId);
        }

        ValidateDocumentAgainstTemplate(template, document);

        var documentRepository = _templateManager.GetDocumentRepository(templateId);

        var id = await _identifierManager.NextIdForTemplateAsync(templateId);

        document.Id = id;

        await documentRepository.WriteAsync(id, document);
    }

    private static void ValidateDocumentAgainstTemplate(Template template, Document document)
    {
        var templateTextFieldsByName = template.TextFieldsByName;
        var templateNumberFieldsByName = template.NumberFieldsByName;
        var templateBoolFieldsByName = template.BoolFieldsByName;

        var documentTextFieldsByFieldName = document.TextFieldValuesByFieldName;
        var documentNumberFieldsByFieldName = document.NumberFieldValuesByFieldName;
        var documentBoolFieldsByFieldName = document.BoolFieldValuesByFieldName;

        foreach (var templateTextField in templateTextFieldsByName)
        {
            if (!documentTextFieldsByFieldName.ContainsKey(templateTextField.Key) && templateTextField.Value.IsRequired)
            {
                throw new TextFieldMissingException(templateTextField.Key);
            }
        }

        foreach (var templateNumberField in templateNumberFieldsByName)
        {
            if (!documentNumberFieldsByFieldName.ContainsKey(templateNumberField.Key) &&
                templateNumberField.Value.IsRequired)
            {
                throw new NumberFieldMissingException(templateNumberField.Key);
            }
        }

        foreach (var templateBoolField in templateBoolFieldsByName)
        {
            if (!documentBoolFieldsByFieldName.ContainsKey(templateBoolField.Key) && templateBoolField.Value.IsRequired)
            {
                throw new BoolFieldMissingException(templateBoolField.Key);
            }
        }
    }

    public async ValueTask<Document?> GetAsync(int templateId, int documentId)
    {
        var template = await _templateManager.GetAsync(templateId);

        if (template is null)
        {
            throw new TemplateNotFoundException(templateId);
        }

        var documentRepository = _templateManager.GetDocumentRepository(templateId);

        return await documentRepository.ReadAsync(documentId);
    }

    public async ValueTask DeleteAsync(int templateId, int documentId)
    {
        var template = await _templateManager.GetAsync(templateId);

        if (template is null)
        {
            throw new TemplateNotFoundException(templateId);
        }

        var documentRepository = _templateManager.GetDocumentRepository(templateId);

        if(!await documentRepository.DeleteAsync(documentId))
        {
            throw new DocumentNotFoundException(documentId);
        }
    }
}
