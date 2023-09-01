using System;
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

    public TemplateService(
        CreateTemplateCommand createTemplateCommand
    )
    {
        _createTemplateCommand = createTemplateCommand;
    }

    public async ValueTask<CreateTemplateResponse> CreateTemplateAsync(CreateTemplateRequest request, CallContext context)
    {
        return await _createTemplateCommand.RunInTransactionAsync(request, context.CancellationToken);
    }

    public ValueTask<GetTemplateResponse> GetTemplateAsync(GetTemplateRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<ListTemplatesResponse> ListTemplatesAsync(ListTemplatesRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }

    public ValueTask<DeleteTemplateResponse> DeleteTemplateAsync(DeleteTemplateRequest request, CallContext context)
    {
        throw new NotImplementedException();
    }
}
