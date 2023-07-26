using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Abstractions;

public interface ITemplateManager
{
    ValueTask CreateAsync(Template template);
    Template? Get(int id);
    IEnumerable<Template> GetAll();
    ValueTask<Template> DeleteAsync(int id);
    ValueTask PerformActionOnLockedTemplateAsync<TArgs>(int id, Func<Template, IRepository<Document>, TArgs, ValueTask> action, TArgs args, CancellationToken cancellationToken = default);
    ValueTask<T> PerformActionOnLockedTemplateAsync<T, TArgs>(int id, Func<Template, IRepository<Document>, TArgs, ValueTask<T>> action, TArgs args, CancellationToken cancellationToken = default);
}
