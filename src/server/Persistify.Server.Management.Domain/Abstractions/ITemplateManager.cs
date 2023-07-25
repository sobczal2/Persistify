using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Server.Persistence.Core.Abstractions;

namespace Persistify.Server.Management.Domain.Abstractions;

public interface ITemplateManager
{
    ValueTask CreateAsync(Template template);
    Template? Get(int id);
    IEnumerable<Template> GetAll();
    ValueTask<Template> DeleteAsync(int id);
    ValueTask PerformActionOnLockedTemplateAsync(int id, Func<Template, IRepository<Document>, ValueTask> action, CancellationToken cancellationToken = default);
    ValueTask<T> PerformActionOnLockedTemplateAsync<T>(int id, Func<Template, IRepository<Document>, ValueTask<T>> action, CancellationToken cancellationToken = default);
}
