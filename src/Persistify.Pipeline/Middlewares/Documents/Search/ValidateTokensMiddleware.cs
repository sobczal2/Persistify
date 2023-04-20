using System.Threading.Tasks;
using FluentValidation;
using Persistify.Pipeline.Contexts.Documents;
using Persistify.Pipeline.Diagnostics;
using Persistify.Pipeline.Exceptions;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Protos;
using Persistify.Tokens;

namespace Persistify.Pipeline.Middlewares.Documents.Search;

[PipelineStep(PipelineStepType.StaticValidation)]
public class ValidateTokensMiddleware : IPipelineMiddleware<SearchDocumentsPipelineContext, SearchDocumentsRequestProto,
    SearchDocumentsResponseProto>
{
    private readonly IValidator<Token<string>> _tokenValidator;

    public ValidateTokensMiddleware(IValidator<Token<string>> tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public Task InvokeAsync(SearchDocumentsPipelineContext context)
    {
        foreach (var token in context.Tokens ?? throw new InternalPipelineError())
            _tokenValidator.ValidateAndThrow(token);

        return Task.CompletedTask;
    }
}