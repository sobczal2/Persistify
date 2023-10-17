using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Persistify.Requests.Templates;
using Persistify.Responses.Templates;
using Persistify.Server.Commands.Templates;
using Persistify.Server.Extensions;
using Persistify.Services;
using ProtoBuf.Grpc;

namespace Persistify.Server.Services;

public class TemplateService : ITemplateService
{
    private readonly CreateTemplateCommand _createTemplateCommand;
    private readonly DeleteTemplateCommand _deleteTemplateCommand;
    private readonly ExistsTemplateCommand _existsTemplateCommand;
    private readonly GetTemplateCommand _getTemplateCommand;
    private readonly ListTemplatesCommand _listTemplatesCommand;

    public TemplateService(
        CreateTemplateCommand createTemplateCommand,
        GetTemplateCommand getTemplateCommand,
        ListTemplatesCommand listTemplatesCommand,
        DeleteTemplateCommand deleteTemplateCommand,
        ExistsTemplateCommand existsTemplateCommand
    )
    {
        _createTemplateCommand = createTemplateCommand;
        _getTemplateCommand = getTemplateCommand;
        _listTemplatesCommand = listTemplatesCommand;
        _deleteTemplateCommand = deleteTemplateCommand;
        _existsTemplateCommand = existsTemplateCommand;
    }

    [Authorize]
    public async ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request,
        CallContext callContext)
    {
        return await _createTemplateCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext callContext)
    {
        return await _getTemplateCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext callContext)
    {
        return _listTemplatesCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request,
        CallContext callContext)
    {
        return await _deleteTemplateCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }

    [Authorize]
    public async ValueTask<ExistsTemplateResponse> ExistsTemplateAsync(ExistsTemplateRequest request,
        CallContext callContext)
    {
        return await _existsTemplateCommand.RunInTransactionAsync(request, callContext.GetClaimsPrincipal(),
            callContext.CancellationToken);
    }
}
