﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;

namespace Persistify.Server.Management.Abstractions.Types;

public interface ITypeManager
{
    ValueTask IndexAsync(int templateId, Document document);
    ValueTask DeleteAsync(int templateId, long documentId);
    ValueTask InitializeForTemplate(Template template);
    ValueTask RemoveForTemplate(Template template);
}

public interface ITypeManager<in TQuery, THit> : ITypeManager
    where TQuery : ITypeManagerQuery
    where THit : ITypeManagerHit
{
    ValueTask<List<THit>> SearchAsync(TQuery query);
}