﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistify.Persistance.Document;

public interface IDocumentStorage
{
    ValueTask AddAsync(string templateName, long documentId, Protos.Documents.Shared.Document document);
    ValueTask<Protos.Documents.Shared.Document?> GetAsync(string templateName, long documentId);
    ValueTask<IEnumerable<Protos.Documents.Shared.Document>> GetAllAsync(string templateName);
    ValueTask<bool> ExistsAsync(string templateName, long documentId);
    ValueTask DeleteAsync(string templateName, long documentId);
    ValueTask AddSpaceForTemplateAsync(string templateName);
    ValueTask DeleteSpaceForTemplateAsync(string templateName);
}