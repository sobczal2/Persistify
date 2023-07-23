﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Templates;

namespace Persistify.Management.Domain.Abstractions;

public interface ITemplateManager
{
    ValueTask CreateAsync(Template template);
    Template? Get(int id);
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    IEnumerable<Template> GetAll();
    ValueTask DeleteAsync(int id);
    ValueTask LockTemplateAsync(int id, CancellationToken cancellationToken = default);
    void UnlockTemplate(int id);
}
