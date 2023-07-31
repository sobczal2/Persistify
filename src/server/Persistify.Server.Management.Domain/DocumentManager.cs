using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Exceptions.Document;
using Persistify.Server.Management.Abstractions.Types;

namespace Persistify.Server.Management.Domain;

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
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (template, repository, doc) =>
            {
                ValidateDocumentAgainstTemplate(template, doc);

                doc.Id = await _documentIdManager.GetNextIdAsync(template.Id);
                await repository.WriteAsync(doc.Id, doc);

                foreach (var typeManager in _typeManagers)
                {
                    await typeManager.IndexAsync(template.Id, doc);
                }

                return doc.Id;
            },
            document);
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

    public async ValueTask<Document?> GetAsync(int templateId, long documentId)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository, docId) => await repository.ReadAsync(docId),
            documentId);
    }

    public async ValueTask DeleteAsync(int templateId, long documentId)
    {
        await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (template, repository, args) =>
            {
                var document = await repository.DeleteAsync(args.documentId) ?? throw new DocumentNotFoundException(args.documentId);

                foreach (var typeManager in args.typeManagers)
                {
                    await typeManager.DeleteAsync(template.Id, document);
                }
            }, (documentId, typeManagers: _typeManagers));
    }

    public ValueTask<IEnumerable<long>> AllIdsAsync(int templateId)
    {
        return ValueTask.FromResult(Array.Empty<long>().AsEnumerable());
    }

    public async ValueTask<List<Document>> GetDocumentsAsync(int templateId, List<long> documentIds)
    {
        return await _templateManager.PerformActionOnLockedTemplateAsync(templateId,
            async (_, repository, docIds) =>
            {
                var documents = new List<Document>(docIds.Count);

                foreach (var docId in docIds)
                {
                    var document = await repository.ReadAsync(docId);

                    if (document != null)
                    {
                        documents.Add(document);
                    }
                }

                return documents;
            }, documentIds);
    }
}
