using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Persistify.Client.LowLevel.Core;
using Persistify.Helpers.Results;

namespace Persistify.Client.HighLevel.Core;

public interface IPersistifyHighLevelClient
{
    IPersistifyLowLevelClient LowLevel { get; }
    Task<Result> InitializeAsync(params Assembly[] assemblies);
    Task<Result<int>> InitializeTemplatesAsync(params Assembly[] assemblies);
    void InitializeConverters(params Assembly[] assemblies);

    Task<Result<int>> AddAsync<TDocument>(TDocument document)
        where TDocument : new();
    Task<Result<TDocument>> GetAsync<TDocument>(int id)
        where TDocument : new();
    Task<Result> DeleteAsync<TDocument>(int id)
        where TDocument : new();
}
