using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneOf;
using Persistify.Indexes.Boolean;
using Persistify.Indexes.Common;
using Persistify.Indexes.Number;
using Persistify.Indexes.Text;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.Indexer)]
public class SearchIndexesInIndexersMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly IIndexer<string> _textIndexer;
    private readonly IIndexer<double> _numberIndexer;
    private readonly IIndexer<bool> _booleanIndexer;

    public SearchIndexesInIndexersMiddleware(
        IIndexer<string> textIndexer,
        IIndexer<double> numberIndexer,
        IIndexer<bool> booleanIndexer
    )
    {
        _textIndexer = textIndexer;
        _numberIndexer = numberIndexer;
        _booleanIndexer = booleanIndexer;
    }

    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var query = context.Request.Query;

        if (query != null)
        {
            context.DocumentIds = (await EvaluateQuery(query, context.TypeDefinition!.Name)).ToArray();
        }
    }

    private async Task<IEnumerable<long>> EvaluateQuery(SearchQuery query, string typeName)
    {
        return query.QueryCase switch
        {
            SearchQuery.QueryOneofCase.And => await EvaluateAndOperator(query.And, typeName),
            SearchQuery.QueryOneofCase.Or => await EvaluateOrOperator(query.Or, typeName),
            SearchQuery.QueryOneofCase.NumberQuery => await EvaluateNumberQuery(query.NumberQuery, typeName),
            SearchQuery.QueryOneofCase.TextQuery => await EvaluateTextQuery(query.TextQuery, typeName),
            SearchQuery.QueryOneofCase.BooleanQuery => await EvaluateBooleanQuery(query.BooleanQuery, typeName),
            _ => throw new InternalPipelineException()
        };
    }

    private async Task<IEnumerable<long>> EvaluateOrOperator(SearchOrOperatorProto orOperator, string typeName)
    {
        var searchTasks = new List<Task<IEnumerable<long>>>();

        foreach (var query in orOperator.Queries)
        {
            switch (query.QueryCase)
            {
                case SearchQuery.QueryOneofCase.And:
                    searchTasks.Add(EvaluateAndOperator(query.And, typeName));
                    break;

                case SearchQuery.QueryOneofCase.Or:
                    searchTasks.Add(EvaluateOrOperator(query.Or, typeName));
                    break;

                case SearchQuery.QueryOneofCase.NumberQuery:
                    searchTasks.Add(EvaluateNumberQuery(query.NumberQuery, typeName));
                    break;

                case SearchQuery.QueryOneofCase.TextQuery:
                    searchTasks.Add(EvaluateTextQuery(query.TextQuery, typeName));
                    break;

                case SearchQuery.QueryOneofCase.BooleanQuery:
                    searchTasks.Add(EvaluateBooleanQuery(query.BooleanQuery, typeName));
                    break;
                case SearchQuery.QueryOneofCase.None:
                    break;
                default:
                    throw new InternalPipelineException();
            }
        }

        var results = await Task.WhenAll(searchTasks);

        return results.SelectMany(x => x).Distinct();
    }


    private async Task<IEnumerable<long>> EvaluateAndOperator(SearchAndOperatorProto andOperator, string typeName)
    {
        var searchTasks = new List<Task<IEnumerable<long>>>();

        foreach (var query in andOperator.Queries)
        {
            switch (query.QueryCase)
            {
                case SearchQuery.QueryOneofCase.And:
                    searchTasks.Add(EvaluateAndOperator(query.And, typeName));
                    break;

                case SearchQuery.QueryOneofCase.Or:
                    searchTasks.Add(EvaluateOrOperator(query.Or, typeName));
                    break;

                case SearchQuery.QueryOneofCase.NumberQuery:
                    searchTasks.Add(EvaluateNumberQuery(query.NumberQuery, typeName));
                    break;

                case SearchQuery.QueryOneofCase.TextQuery:
                    searchTasks.Add(EvaluateTextQuery(query.TextQuery, typeName));
                    break;

                case SearchQuery.QueryOneofCase.BooleanQuery:
                    searchTasks.Add(EvaluateBooleanQuery(query.BooleanQuery, typeName));
                    break;
                case SearchQuery.QueryOneofCase.None:
                    break;
                default:
                    throw new InternalPipelineException();
            }
        }

        var results = await Task.WhenAll(searchTasks);

        if (results.Length == 0)
        {
            return Enumerable.Empty<long>();
        }

        var commonIds = results[0].Distinct();

        for (var i = 1; i < results.Length; i++)
        {
            commonIds = commonIds.Intersect(results[i].Distinct());
        }

        return commonIds;
    }

    private async Task<IEnumerable<long>> EvaluateNumberQuery(SearchNumberQueryProto numberQuery, string typeName)
    {
        var documentIds = await _numberIndexer.SearchAsync(new NumberSearchPredicate
        {
            TypeName = typeName,
            Path = numberQuery.Path,
            Min = numberQuery.Min,
            Max = numberQuery.Max,
        });

        return documentIds.Distinct();
    }

    private async Task<IEnumerable<long>> EvaluateTextQuery(SearchTextQueryProto textQuery, string typeName)
    {
        var documentIds = await _textIndexer.SearchAsync(new TextSearchPredicate
        {
            TypeName = typeName,
            Path = textQuery.Path,
            Value = textQuery.Value,
            CaseSensitive = textQuery.CaseSensitive,
            Exact = textQuery.Exact,
        });

        return documentIds.Distinct();
    }

    private async Task<IEnumerable<long>> EvaluateBooleanQuery(SearchBooleanQueryProto booleanQuery, string typeName)
    {
        var documentIds = await _booleanIndexer.SearchAsync(new BooleanSearchPredicate
        {
            TypeName = typeName,
            Path = booleanQuery.Path,
            Value = booleanQuery.Value
        });

        return documentIds.Distinct();
    }
}