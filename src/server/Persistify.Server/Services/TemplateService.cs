using System.Threading.Tasks;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Templates;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly CreateTemplateCommand _createTemplateCommand;
    private readonly DeleteTemplateCommand _deleteTemplateCommand;
    private readonly GetTemplateCommand _getTemplateCommand;
    private readonly ListTemplatesCommand _listTemplatesCommand;

    public TemplateService(
        CreateTemplateCommand createTemplateCommand,
        GetTemplateCommand getTemplateCommand,
        ListTemplatesCommand listTemplatesCommand,
        DeleteTemplateCommand deleteTemplateCommand
    )
    {
        _createTemplateCommand = createTemplateCommand;
        _getTemplateCommand = getTemplateCommand;
        _listTemplatesCommand = listTemplatesCommand;
        _deleteTemplateCommand = deleteTemplateCommand;
    }

    public async ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request,
        CallContext context)
    {
        return await _createTemplateCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public async ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext context)
    {
        return await _getTemplateCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext context)
    {
        return _listTemplatesCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public async ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request,
        CallContext context)
    {
        return await _deleteTemplateCommand.RunInTransactionAsync(request, context.CancellationToken);
    }
}
