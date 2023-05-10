using System.Collections.Generic;
using System.Threading.Tasks;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Validators;
using Persistify.Validators.Core;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.DynamicValidation)]
public class ValidateQueryAgainstTypeMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext,
    SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    public async Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var typeDefinition = context.TypeDefinition ?? throw new InternalPipelineException();
        var fields = new Dictionary<string, FieldDefinitionProto>();
        foreach (var field in typeDefinition.Fields) fields.Add(field.Path, field);
        ValidateQuery(context.Request.Query, fields);
    }

    private void ValidateQuery(SearchQueryProto searchQuery, Dictionary<string, FieldDefinitionProto> fields)
    {
        switch (searchQuery.QueryCase)
        {
            case SearchQueryProto.QueryOneofCase.OrOperator:
                ValidateOrOperator(searchQuery.OrOperator, fields);
                break;
            case SearchQueryProto.QueryOneofCase.AndOperator:
                ValidateAndOperator(searchQuery.AndOperator, fields);
                break;
            case SearchQueryProto.QueryOneofCase.NumberQuery:
                ValidateNumberQuery(searchQuery.NumberQuery, fields);
                break;
            case SearchQueryProto.QueryOneofCase.TextQuery:
                ValidateTextQuery(searchQuery.TextQuery, fields);
                break;
            case SearchQueryProto.QueryOneofCase.BooleanQuery:
                ValidateBooleanQuery(searchQuery.BooleanQuery, fields);
                break;
        }
    }

    private void ValidateOrOperator(SearchOrOperatorProto searchOrOperator,
        Dictionary<string, FieldDefinitionProto> fields)
    {
        foreach (var searchQuery in searchOrOperator.Queries) ValidateQuery(searchQuery, fields);
    }

    private void ValidateAndOperator(SearchAndOperatorProto searchAndOperator,
        Dictionary<string, FieldDefinitionProto> fields)
    {
        foreach (var searchQuery in searchAndOperator.Queries) ValidateQuery(searchQuery, fields);
    }

    private void ValidateNumberQuery(SearchNumberQueryProto searchNumberQuery,
        Dictionary<string, FieldDefinitionProto> fields)
    {
        if (fields.TryGetValue(searchNumberQuery.Path, out var field))
        {
            if (field.Type != FieldTypeProto.Number)
                throw new ValidationException(new[] { ValidationFailures.FieldTypeInvalid });
        }
        else
        {
            throw new ValidationException(new[] { ValidationFailures.FieldNotFound });
        }
    }

    private void ValidateTextQuery(SearchTextQueryProto searchTextQuery,
        Dictionary<string, FieldDefinitionProto> fields)
    {
        if (fields.TryGetValue(searchTextQuery.Path, out var field))
        {
            if (field.Type != FieldTypeProto.Text)
                throw new ValidationException(new[] { ValidationFailures.FieldTypeInvalid });
        }
        else
        {
            throw new ValidationException(new[] { ValidationFailures.FieldNotFound });
        }
    }

    private void ValidateBooleanQuery(SearchBooleanQueryProto searchBooleanQuery,
        Dictionary<string, FieldDefinitionProto> fields)
    {
        if (fields.TryGetValue(searchBooleanQuery.Path, out var field))
        {
            if (field.Type != FieldTypeProto.Boolean)
                throw new ValidationException(new[] { ValidationFailures.FieldTypeInvalid });
        }
        else
        {
            throw new ValidationException(new[] { ValidationFailures.FieldNotFound });
        }
    }
}