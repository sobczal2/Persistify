using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators.Documents;

namespace Persistify.Pipeline.Middlewares.Documents;

public class ValidateTypeNameMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto, SearchDocumentsResponseProto>
{
    private readonly ITypeStore _typeStore;

    public ValidateTypeNameMiddleware(
        ITypeStore typeStore
        )
    {
        _typeStore = typeStore;
    }
    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        var typeExists = _typeStore.Exists(context.Request.TypeName);
        if (!typeExists)
            throw new ValidationException(new[]
            {
                new ValidationFailure("TypeName", "Type does not exist")
                {
                    ErrorCode = DocumentErrorCodes.TypeNotFound
                }
            });
        
        context.PreviousPipelineStep = PipelineStep.TypeStore;
        
        return Task.CompletedTask;
    }
}