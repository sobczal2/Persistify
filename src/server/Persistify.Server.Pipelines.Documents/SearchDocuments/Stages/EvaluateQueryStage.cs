using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.SearchModes;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Responses.Documents;
using Persistify.Server.Management.Abstractions;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Abstractions.Types;
using Persistify.Server.Management.Types.Bool;
using Persistify.Server.Management.Types.Fts;
using Persistify.Server.Management.Types.Fts.Queries;
using Persistify.Server.Management.Types.Number;
using Persistify.Server.Management.Types.Number.Queries;
using Persistify.Server.Management.Types.Shared;
using Persistify.Server.Management.Types.Text;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

public class EvaluateQueryStage : PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
    SearchDocumentsResponse>
{
    private readonly ITypeManager<TextManagerQuery, TextManagerHit> _textManager;
    private readonly ITypeManager<FtsManagerQuery, FtsManagerHit> _ftsManager;
    private readonly ITypeManager<NumberManagerQuery, NumberManagerHit> _numberManager;
    private readonly ITypeManager<BoolManagerQuery, BoolManagerHit> _boolManager;
    private readonly IDocumentManager _documentManager;
    private const string StageName = "EvaluateQuery";
    public override string Name => StageName;

    public EvaluateQueryStage(
        ILogger<EvaluateQueryStage> logger,
        ITypeManager<TextManagerQuery, TextManagerHit> textManager,
        ITypeManager<FtsManagerQuery, FtsManagerHit> ftsManager,
        ITypeManager<NumberManagerQuery, NumberManagerHit> numberManager,
        ITypeManager<BoolManagerQuery, BoolManagerHit> boolManager,
        IDocumentManager documentManager
    ) : base(
        logger
    )
    {
        _textManager = textManager;
        _ftsManager = ftsManager;
        _numberManager = numberManager;
        _boolManager = boolManager;
        _documentManager = documentManager;
    }

    public override async ValueTask<Result> ProcessAsync(SearchDocumentsPipelineContext context)
    {
        context.DocumentScores = await EvaluateSearchNode(context.Request.SearchNode, context.TemplateId);

        return Result.Success;
    }

    public override ValueTask<Result> RollbackAsync(SearchDocumentsPipelineContext context)
    {
        return ValueTask.FromResult(Result.Success);
    }

    private async ValueTask<List<DocumentScore>> EvaluateSearchNode(SearchNode searchNode, int templateId)
    {
        switch (searchNode)
        {
            case TextSearchNode node:
                return await EvaluateTextSearchNode(node, templateId);
            case FtsSearchNode node:
                return await EvaluateFtsSearchNode(node, templateId);
            case NumberSearchNode node:
                return await EvaluateNumberSearchNode(node, templateId);
            case NumberRangeSearchNode node:
                return await EvaluateNumberRangeSearchNode(node, templateId);
            case BoolSearchNode node:
                return await EvaluateBoolSearchNode(node, templateId);
            case AndSearchNode node:
                return await EvaluateAndSearchNode(node, templateId);
            case OrSearchNode node:
                return await EvaluateOrSearchNode(node, templateId);
            case NotSearchNode node:
                return await EvaluateNotSearchNode(node, templateId);
            default:
                throw new PipelineException();
        }
    }

    private async ValueTask<List<DocumentScore>> EvaluateTextSearchNode(TextSearchNode node, int templateId)
    {
        var query = new TextManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
        var hits = await _textManager.SearchAsync(query);
        var documentIds = new List<DocumentScore>(hits.Count);
        foreach (var hit in hits)
        {
            documentIds.Add(new DocumentScore(hit.DocumentId, 1));
        }

        return documentIds;
    }

    private async ValueTask<List<DocumentScore>> EvaluateFtsSearchNode(FtsSearchNode node, int templateId)
    {
        FtsManagerQuery query;
        switch (node.Mode)
        {
            case FtsSearchMode.Prefix:
                query = new PrefixFtsManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
                break;
            case FtsSearchMode.Suffix:
                query = new SuffixFtsManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
                break;
            case FtsSearchMode.Exact:
                query = new ExactFtsManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
                break;
            case FtsSearchMode.Contains:
                query = new ContainsFtsManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var hits = await _ftsManager.SearchAsync(query);
        var documentIds = new List<DocumentScore>(hits.Count);
        foreach (var hit in hits)
        {
            documentIds.Add(new DocumentScore(hit.DocumentId, hit.Score));
        }

        return documentIds;
    }

    private async ValueTask<List<DocumentScore>> EvaluateNumberSearchNode(NumberSearchNode node, int templateId)
    {
        NumberManagerQuery query;
        switch (node.Mode)
        {
            case NumberSearchMode.Equal:
                query = new EqualNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            case NumberSearchMode.NotEqual:
                query = new NotEqualNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            case NumberSearchMode.GreaterThan:
                query = new GreaterThanNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            case NumberSearchMode.GreaterThanOrEqual:
                query = new GreaterThanOrEqualNumberManagerQuery(
                    new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
                break;
            case NumberSearchMode.LessThan:
                query = new LessThanNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            case NumberSearchMode.LessThanOrEqual:
                query = new LessThanOrEqualNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName),
                    node.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var hits = await _numberManager.SearchAsync(query);
        var documentIds = new List<DocumentScore>(hits.Count);
        foreach (var hit in hits)
        {
            documentIds.Add(new DocumentScore(hit.DocumentId, 1));
        }

        return documentIds;
    }

    private async ValueTask<List<DocumentScore>> EvaluateNumberRangeSearchNode(NumberRangeSearchNode node,
        int templateId)
    {
        var query = new RangeNumberManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Min,
            node.Max, node.IncludeMin, node.IncludeMax);
        var hits = await _numberManager.SearchAsync(query);
        var documentIds = new List<DocumentScore>(hits.Count);
        foreach (var hit in hits)
        {
            documentIds.Add(new DocumentScore(hit.DocumentId, 1));
        }

        return documentIds;
    }

    private async ValueTask<List<DocumentScore>> EvaluateBoolSearchNode(BoolSearchNode node, int templateId)
    {
        var query = new BoolManagerQuery(new TemplateFieldIdentifier(templateId, node.FieldName), node.Value);
        var hits = await _boolManager.SearchAsync(query);
        var documentIds = new List<DocumentScore>(hits.Count);
        foreach (var hit in hits)
        {
            documentIds.Add(new DocumentScore(hit.DocumentId, 1));
        }

        return documentIds;
    }

    private async ValueTask<List<DocumentScore>> EvaluateAndSearchNode(AndSearchNode node, int templateId)
    {
        var lists = new List<DocumentScore>[node.Nodes.Count];
        for (var i = 0; i < node.Nodes.Count; i++)
        {
            lists[i] = await EvaluateSearchNode(node.Nodes[i], templateId);
        }

        return IntersectSortedLists(lists);
    }

    private async ValueTask<List<DocumentScore>> EvaluateOrSearchNode(OrSearchNode node, int templateId)
    {
        var lists = new List<DocumentScore>[node.Nodes.Count];
        for (var i = 0; i < node.Nodes.Count; i++)
        {
            lists[i] = await EvaluateSearchNode(node.Nodes[i], templateId);
        }

        return MergeSortedLists(lists);
    }

    private ValueTask<List<DocumentScore>> EvaluateNotSearchNode(NotSearchNode node, int templateId)
    {
        throw new NotImplementedException();
    }

    // all lists are sorted
    private static List<DocumentScore> IntersectSortedLists(List<DocumentScore>[] lists)
    {
        var intersection = new List<DocumentScore>();
        var enumerators = new List<IEnumerator<DocumentScore>>(lists.Length);

        foreach (var list in lists)
        {
            enumerators.Add(list.GetEnumerator());
        }

        foreach (var enumerator in enumerators)
        {
            enumerator.MoveNext();
        }

        while (true)
        {
            if (EnumeratorsPointToSameDocument(enumerators))
            {
                intersection.Add(MergeDocumentScores(enumerators));
                foreach (var enumerator in enumerators)
                {
                    if (!enumerator.MoveNext()) return intersection;
                }
            }
            else
            {
                if (!MoveNextLowestIdEnumerators(enumerators)) return intersection;
            }
        }
    }

    private static bool EnumeratorsPointToSameDocument(List<IEnumerator<DocumentScore>> enumerators)
    {
        var documentIds = new HashSet<long>();
        foreach (var enumerator in enumerators)
        {
            documentIds.Add(enumerator.Current.DocumentId);
        }

        return documentIds.Count == 1;
    }

    private static DocumentScore MergeDocumentScores(List<IEnumerator<DocumentScore>> enumerators)
    {
        var documentId = enumerators[0].Current.DocumentId;
        var score = 0f;
        foreach (var enumerator in enumerators)
        {
            score += enumerator.Current.Score;
        }

        return new DocumentScore(documentId, score);
    }

    private static bool MoveNextLowestIdEnumerators(List<IEnumerator<DocumentScore>> enumerators)
    {
        var minId = long.MaxValue;
        foreach (var enumerator in enumerators)
        {
            if (enumerator.Current.DocumentId < minId)
            {
                minId = enumerator.Current.DocumentId;
            }
        }

        var moved = false;
        foreach (var enumerator in enumerators)
        {
            if (enumerator.Current.DocumentId == minId)
            {
                moved = enumerator.MoveNext() || moved;
            }
        }

        return moved;
    }

    public static List<DocumentScore> MergeSortedLists(List<DocumentScore>[] lists)
    {
        var union = new List<DocumentScore>();
        var enumerators = new List<List<DocumentScore>.Enumerator>(lists.Length);

        foreach (var list in lists)
        {
            var enumerator = list.GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerators.Add(enumerator);
            }
        }

        foreach (var enumerator in enumerators)
        {
            enumerator.MoveNext();
        }

        while (true)
        {
            long minVal = long.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < enumerators.Count; i++)
            {
                if (enumerators[i].Current.DocumentId < minVal)
                {
                    minVal = enumerators[i].Current.DocumentId;
                    minIndex = i;
                }
            }

            if (minIndex == -1)
                break;

            if (union.Count == 0 || union[^1].DocumentId != enumerators[minIndex].Current.DocumentId )
            {
                union.Add(enumerators[minIndex].Current);
            }
            else if (union[^1].DocumentId == enumerators[minIndex].Current.DocumentId)
            {
                union[^1] = new DocumentScore(union[^1].DocumentId,
                    union[^1].Score + enumerators[minIndex].Current.Score);
            }

            if (!enumerators[minIndex].MoveNext())
            {
                enumerators.RemoveAt(minIndex);
            }
        }

        return union;
    }
}
