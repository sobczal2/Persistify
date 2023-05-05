using System.Threading.Tasks;
using FluentValidation;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents.Index;

[PipelineStep(PipelineStepType.StaticValidation)]
public class ValidateTokensMiddleware : IPipelineMiddleware<IndexDocumentPipelineContext, IndexDocumentRequestProto,
    IndexDocumentResponseProto>
{
    private readonly IValidator<Token<string>> _tokenValidator;

    public ValidateTokensMiddleware(IValidator<Token<string>> tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public Task InvokeAsync(IndexDocumentPipelineContext context)
    {
        foreach (var token in context.TextTokens ?? throw new InternalPipelineException())
            _tokenValidator.ValidateAndThrow(token);

        return Task.CompletedTask;
    }
}