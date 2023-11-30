using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Helpers.Collections;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.CommandHandlers.Common;
using Persistify.Server.Domain.Templates;
using Persistify.Server.Domain.Users;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Mappers.Templates;

namespace Persistify.Server.CommandHandlers.Templates;

public sealed class CreateTemplateRequestHandler
    : RequestHandler<CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private readonly ITemplateManager _templateManager;
    private Template? _template;

    public CreateTemplateRequestHandler(
        IRequestHandlerContext<CreateTemplateRequest, CreateTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager,
        IPresetAnalyzerManager presetAnalyzerManager
    )
        : base(requestHandlerContext)
    {
        _templateManager = templateManager;
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override async ValueTask RunAsync(
        CreateTemplateRequest request,
        CancellationToken cancellationToken
    )
    {
        _template = new Template
        {
            Name = request.TemplateName,
            Fields = await request.Fields.ListSelectAsync(
                x =>
                    x.ToDomain(
                        async name =>
                            (await _presetAnalyzerManager.GetAsync(name))?.Analyzer
                            ?? throw new InternalPersistifyException(nameof(CreateTemplateRequest))
                    )
            )
        };

        _templateManager.Add(_template);
    }

    protected override CreateTemplateResponse GetResponse()
    {
        if (_template is null)
        {
            throw new InternalPersistifyException(nameof(CreateTemplateResponse));
        }

        return new CreateTemplateResponse();
    }

    protected override TransactionDescriptor GetTransactionDescriptor(
        CreateTemplateRequest request
    )
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _presetAnalyzerManager },
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(
        CreateTemplateRequest request
    )
    {
        return Permission.TemplateWrite;
    }
}
