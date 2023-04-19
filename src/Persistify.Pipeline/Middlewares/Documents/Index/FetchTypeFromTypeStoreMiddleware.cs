using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Types;
using Persistify.Validators.Documents;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

public class FetchTypeFromTypeStoreMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly ITypeStore _typeStore;

    public FetchTypeFromTypeStoreMiddleware(
        ITypeStore typeStore
    )
    {
        _typeStore = typeStore;
    }

    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        try
        {
            context.TypeDefinition = _typeStore.Get(context.Request.TypeName);
        }
        catch (TypeNotFoundException)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("TypeName", "Type not found")
                {
                    ErrorCode = DocumentErrorCodes.TypeNotFound
                }
            });
        }

        return Task.CompletedTask;
    }
}