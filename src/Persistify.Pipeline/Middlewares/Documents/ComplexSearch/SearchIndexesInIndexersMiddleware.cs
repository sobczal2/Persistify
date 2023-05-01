using System.Collections;
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
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Middlewares.Documents.ComplexSearch;

[PipelineStep(PipelineStepType.Indexer)]
public class SearchIndexesInIndexersMiddleware : IPipelineMiddleware<ComplexSearchDocumentsPipelineContext,
    ComplexSearchDocumentsRequestProto, ComplexSearchDocumentsResponseProto>
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

    public async Task InvokeAsync(ComplexSearchDocumentsPipelineContext context)
    {
        if (context.Request.Or != null)
        {
            context.DocumentIds = await EvaluateOrOperator(context.Request.Or, context.TypeDefinition!.Name);
        }
        
        if (context.Request.And != null)
        {
            context.DocumentIds = await EvaluateAndOperator(context.Request.And, context.TypeDefinition!.Name);
        }
        
        if (context.Request.NumberQuery != null)
        {
            context.DocumentIds = await EvaluateNumberQuery(context.Request.NumberQuery, context.TypeDefinition!.Name);
        }
        
        if (context.Request.TextQuery != null)
        {
            context.DocumentIds = await EvaluateTextQuery(context.Request.TextQuery, context.TypeDefinition!.Name);
        }
        
        if (context.Request.BooleanQuery != null)
        {
            context.DocumentIds = await EvaluateBooleanQuery(context.Request.BooleanQuery, context.TypeDefinition!.Name);
        }
    }

    private async Task<IEnumerable<long>> EvaluateOrOperator(ComplexSearchOrOperatorProto orOperator, string typeName)
    {
        var groupedResults = new List<List<long>>();

        foreach (var queryOperator in orOperator.And)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in orOperator.Or)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in orOperator.NumberQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in orOperator.TextQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in orOperator.BooleanQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        return groupedResults.SelectMany(x => x).Distinct();
    }

    private async Task<IEnumerable<long>> EvaluateAndOperator(ComplexSearchAndOperatorProto andOperator, string typeName)
    {
        var groupedResults = new List<List<long>>();

        foreach (var queryOperator in andOperator.And)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in andOperator.Or)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in andOperator.NumberQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in andOperator.TextQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }

        foreach (var queryOperator in andOperator.BooleanQuery)
        {
            groupedResults.Add((await Evaluate(queryOperator, typeName)).ToList());
        }
        
        if (groupedResults.Count == 0)
        {
            return Enumerable.Empty<long>();
        }

        IEnumerable<long> commonIds = groupedResults[0];

        for (int i = 1; i < groupedResults.Count; i++)
        {
            commonIds = commonIds.Intersect(groupedResults[i]);
        }

        return commonIds;
    }

    private async Task<IEnumerable<long>> EvaluateNumberQuery(ComplexSearchNumberQueryProto numberQuery,
        string typeName)
    {
        var indexes = await _numberIndexer.SearchAsync(new NumberSearchPredicate()
            {
                Min = numberQuery.Min,
                Max = numberQuery.Max
            },
            typeName);

        return indexes.Where(x => x.Path == typeName).Select(x => x.Id).Distinct();
    }

    private async Task<IEnumerable<long>> EvaluateTextQuery(ComplexSearchTextQueryProto textQuery, string typeName)
    {
        var indexes = await _textIndexer.SearchAsync(new TextSearchPredicate()
            {
                Value = textQuery.Value
            },
            typeName);

        return indexes.Where(x => x.Path == typeName).Select(x => x.Id).Distinct();
    }

    private async Task<IEnumerable<long>> EvaluateBooleanQuery(ComplexSearchBooleanQueryProto booleanQuery,
        string typeName)
    {
        var indexes = await _booleanIndexer.SearchAsync(new BooleanSearchPredicate()
            {
                Value = booleanQuery.Value
            },
            typeName);

        return indexes.Where(x => x.Path == typeName).Select(x => x.Id).Distinct();
    }

    private async Task<IEnumerable<long>> Evaluate(
        OneOf<ComplexSearchOrOperatorProto, ComplexSearchAndOperatorProto, ComplexSearchNumberQueryProto,
            ComplexSearchTextQueryProto, ComplexSearchBooleanQueryProto> query, string typeName)
    {
        return await query.Match(
            orOperator => EvaluateOrOperator(orOperator, typeName),
            andOperator => EvaluateAndOperator(andOperator, typeName),
            numberQuery => EvaluateNumberQuery(numberQuery, typeName),
            textQuery => EvaluateTextQuery(textQuery, typeName),
            booleanQuery => EvaluateBooleanQuery(booleanQuery, typeName)
        );
    }
}