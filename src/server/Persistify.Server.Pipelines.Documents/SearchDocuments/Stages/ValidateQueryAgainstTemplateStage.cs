using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Domain.Templates;
using Persistify.Helpers.ErrorHandling;
using Persistify.Requests.Documents;
using Persistify.Requests.Search;
using Persistify.Responses.Documents;
using Persistify.Server.Pipelines.Common;
using Persistify.Server.Pipelines.Exceptions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Documents.SearchDocuments.Stages;

public class ValidateQueryAgainstTemplateStage : PipelineStage<SearchDocumentsPipelineContext, SearchDocumentsRequest,
    SearchDocumentsResponse>
{
    private const string StageName = "ValidateQueryAgainstTemplateStage";
    public override string Name => StageName;

    public ValidateQueryAgainstTemplateStage(ILogger<ValidateQueryAgainstTemplateStage> logger) : base(logger)
    {
    }

    public override ValueTask ProcessAsync(SearchDocumentsPipelineContext context)
    {
        ValidateSearchNode(
            context.Request.SearchNode,
            context.Template ?? throw new PipelineException(),
            "SearchDocumentsRequest.SearchNode");

        return ValueTask.CompletedTask;
    }

    private void ValidateSearchNode(SearchNode searchNode, Template template, string errorPrefix)
    {
        switch (searchNode)
        {
            case TextSearchNode node:
                if (!template.TextFieldsByName.ContainsKey(node.FieldName))
                    throw new ValidationException(errorPrefix,
                        $"Field {node.FieldName} is not defined in template {template.Name}");
                break;
            case FtsSearchNode node:
                if (!template.TextFieldsByName.ContainsKey(node.FieldName))
                    throw new ValidationException(errorPrefix,
                        $"Field {node.FieldName} is not defined in template {template.Name}");
                break;
            case NumberSearchNode node:
                if (!template.NumberFieldsByName.ContainsKey(node.FieldName))
                    throw new ValidationException(errorPrefix,
                        $"Field {node.FieldName} is not defined in template {template.Name}");
                break;
            case NumberRangeSearchNode node:
                if (!template.NumberFieldsByName.ContainsKey(node.FieldName))
                    throw new ValidationException(errorPrefix,
                        $"Field {node.FieldName} is not defined in template {template.Name}");
                break;
            case BoolSearchNode node:
                if (!template.BoolFieldsByName.ContainsKey(node.FieldName))
                    throw new ValidationException(errorPrefix,
                        $"Field {node.FieldName} is not defined in template {template.Name}");
                break;
            case AndSearchNode node:
                for (var i = 0; i < node.Nodes.Count; i++)
                {
                    ValidateSearchNode(node.Nodes[i], template, $"{errorPrefix}.Nodes[{i}]");
                }

                break;
            case OrSearchNode node:
                for (var i = 0; i < node.Nodes.Count; i++)
                {
                    ValidateSearchNode(node.Nodes[i], template, $"{errorPrefix}.Nodes[{i}]");
                }

                break;
            case NotSearchNode node:
                ValidateSearchNode(node.Node, template, $"{errorPrefix}.Node");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(searchNode));
        }
    }
}
