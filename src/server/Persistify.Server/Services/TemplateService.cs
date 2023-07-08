using System;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Protos.Templates;
using Serilog;

namespace Persistify.Server.Services;

public class TemplateService : Protos.Templates.TemplateService.TemplateServiceBase
{
    private readonly ILogger _logger;

    public TemplateService(ILogger logger)
    {
        _logger = logger;
    }
    public override Task<AddTemplateResponse> Add(AddTemplateRequest request, ServerCallContext context)
    {
        Console.WriteLine(context.ResponseTrailers.GetValue("X-Correlation-ID"));
        _logger.Information("AddTemplateRequest received at {Timestamp}", DateTime.UtcNow);
        return Task.FromResult(new AddTemplateResponse());
    }
}
