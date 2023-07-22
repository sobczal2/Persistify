using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Persistify.Domain.Templates;

namespace Persistify.Management.Domain.Abstractions;

public interface ITemplateManager
{
    ValueTask CreateAsync(Template template);
    ValueTask<Template?> GetAsync(int id);
    ValueTask<IEnumerable<Template>> GetAllAsync();
    ValueTask DeleteAsync(int id);
}
