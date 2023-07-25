using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Domain.Abstractions;
using Persistify.Server.Management.Domain.Exceptions;
using Persistify.Server.Management.Domain.Exceptions.Document;
using Persistify.Server.Management.Types.Abstractions;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Implementations;

public class DocumentManager : IDocumentManager
{
    private readonly ITemplateManager _templateManager;
    private readonly IDocumentIdManager _documentIdManager;
    private readonly IEnumerable<ITypeManager> _typeManagers;

    public DocumentManager(
        ITemplateManager templateManager,
        IDocumentIdManager documentIdManager,
        IEnumerable<ITypeManager> typeManagers
    )
    {
        _templateManager = templateManager;
        _documentIdManager = documentIdManager;
        _typeManagers = typeManagers;
    }

    public async ValueTask<long> IndexAsync(int templateId, Document document)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId, async (template, repository) =>
        {
            ValidateDocumentAgainstTemplate(template, document);

            document.Id = await _documentIdManager.GetNextId(templateId);
            await repository.WriteAsync(document.Id, document);

            foreach (var typeManager in _typeManagers)
            {
                await typeManager.IndexAsync(templateId, document);
            }

            return document.Id;
        });
    }

    private void ValidateDocumentAgainstTemplate(Template template, Document document)
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
            if (!documentNumberFieldsByFieldName.ContainsKey(templateNumberField.Key) && templateNumberField.Value.IsRequired)
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

    public async ValueTask<Document?> GetAsync(int templateId, long documentId)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository) => await repository.ReadAsync(documentId));
    }

    public async ValueTask DeleteAsync(int templateId, long documentId)
    {
        await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository) =>
            {
                await repository.DeleteAsync(documentId);

                foreach (var typeManager in _typeManagers)
                {
                    await typeManager.DeleteAsync(templateId, documentId);
                }
            });
    }
}
