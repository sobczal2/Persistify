using System.Threading.Tasks;
using FluentValidation;
using Persistify.Pipeline.Contexts.Abstractions;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Stores.Documents;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents;

public class ValidateTokensMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto, IndexDocumentResponseProto>
{
    private readonly IValidator<Token> _tokenValidator;

    public ValidateTokensMiddleware(IValidator<Token> tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }
    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        foreach (var token in context.Tokens ?? throw new InternalPipelineError())
        {
            _tokenValidator.ValidateAndThrow(token);
        }
        
        context.PreviousPipelineStep = PipelineStep.Validation;

        return Task.CompletedTask;
    }
}