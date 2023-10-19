using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
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

namespace Persistify.Server.CommandHandlers.Templates;

public sealed class CreateTemplateRequestHandler : RequestHandler<CreateTemplateRequest, CreateTemplateResponse>
{
    private readonly ITemplateManager _templateManager;
    private readonly IPresetAnalyzerManager _presetAnalyzerManager;
    private Template? _template;

    public CreateTemplateRequestHandler(
        IRequestHandlerContext<CreateTemplateRequest, CreateTemplateResponse> requestHandlerContext,
        ITemplateManager templateManager,
        IPresetAnalyzerManager presetAnalyzerManager
    ) : base(
        requestHandlerContext
    )
    {
        _templateManager = templateManager;
        _presetAnalyzerManager = presetAnalyzerManager;
    }

    protected override async ValueTask RunAsync(CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        var templateFields = new List<Field>();
        foreach (var field in request.Fields)
        {
            switch (field)
            {
                case BoolFieldDto boolFieldDto:
                    templateFields.Add(new BoolField { Name = boolFieldDto.Name, Required = boolFieldDto.Required, });
                    break;
                case NumberFieldDto numberFieldDto:
                    templateFields.Add(new NumberField
                    {
                        Name = numberFieldDto.Name, Required = numberFieldDto.Required,
                    });
                    break;
                case TextFieldDto textFieldDto:
                    Analyzer analyzer = textFieldDto.Analyzer switch
                    {
                        FullAnalyzerDto fullAnalyzerDescriptorDto =>
                            new Analyzer
                            {
                                CharacterFilterNames = fullAnalyzerDescriptorDto.CharacterFilterNames,
                                TokenizerName = fullAnalyzerDescriptorDto.TokenizerName,
                                TokenFilterNames = fullAnalyzerDescriptorDto.TokenFilterNames
                            },
                        PresetNameAnalyzerDto presetAnalyzerDescriptorDto =>
                            (await _presetAnalyzerManager.GetAsync(presetAnalyzerDescriptorDto.PresetName))?.Analyzer ??
                            throw new InternalPersistifyException(),
                        _ => throw new InternalPersistifyException()
                    };
                    templateFields.Add(new TextField
                    {
                        Name = textFieldDto.Name, Required = textFieldDto.Required, Analyzer = analyzer
                    });
                    break;
                default:
                    throw new InternalPersistifyException();
            }
        }

        _template = new Template { Name = request.TemplateName, Fields = templateFields };

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

    protected override TransactionDescriptor GetTransactionDescriptor(CreateTemplateRequest request)
    {
        return new TransactionDescriptor(
            false,
            new List<IManager> { _presetAnalyzerManager },
            new List<IManager> { _templateManager }
        );
    }

    protected override Permission GetRequiredPermission(CreateTemplateRequest request)
    {
        return Permission.TemplateWrite;
    }
}
